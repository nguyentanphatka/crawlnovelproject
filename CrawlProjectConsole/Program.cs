using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CrawlProjectConsole;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;


// // From Web
// var url = "https://www.kiemhieptruyen.com/muc-luc/";
// var web = new HtmlWeb();
// web.OverrideEncoding = Encoding.UTF8;
// var doc = web.Load(url);
//
// // foreach (HtmlNode row in doc.DocumentNode.SelectNodes("//a[@class='a1']")) 
// var data = doc.DocumentNode.SelectNodes("menu-ngang");


var _context = new StoryDbContext();
// download html
var url = @"https://www.kiemhieptruyen.com/";
var novelUrl =  @"https://www.kiemhieptruyen.com/muc-luc/";

await GetNovel(novelUrl);

var novels = await _context.Stories.Select(story => new
{
    story.Id,
    story.Link
}).ToListAsync();

if (novels.Any())
{
    foreach (var novel in novels)
    {
       await GetChapter(novel.Id,novel.Link);
    }
}
// context.SaveChanges();
Console.WriteLine("Finish");
Console.ReadKey();

async Task GetNovel(string novelUrl = @"https://www.kiemhieptruyen.com/muc-luc/")
{
    var result = new List<Story>();
    // get html string
    var html = new WebClient().DownloadString(novelUrl);

    // init the html doc
    var doc = new HtmlDocument();
    doc.LoadHtml(html);
    
    // get novel nodes by xpath
    var novelNodes = doc.DocumentNode.SelectNodes("//ul[@class='khungul']/li");

    if (novelNodes == null)
        return;
    var existNovelNames = await _context.Stories.Select(story => story.Title).ToListAsync();
    foreach (var novelNode in novelNodes)
    {
        var novel = new Story();
        var titleNode = novelNode.SelectSingleNode("./*[@class='a1']");
        if(titleNode==null)
            continue;
        novel.Title = titleNode.InnerText?.Trim()??"blank";
        if(!string.Equals(novel.Link,"blank") &&  existNovelNames.Any(novelName =>novelName==novel.Title ))
            continue;
        novel.Link = titleNode.GetAttributeValue("href", "");
        
        // get author
        var crawlLink = $"{url}{novel.Link}";
        var chapterHtml = new WebClient().DownloadString(crawlLink);
        var chapterDoc = new HtmlDocument();
        chapterDoc.LoadHtml(chapterHtml);
        var novelAuthor = doc.DocumentNode.SelectSingleNode("//meta[@name='author']");
        var authorName = novelAuthor?.GetAttributeValue("content","anonymous");
        novel.Author = authorName??"anonymous";
        
        result.Add(novel);
    }
    _context.Stories.AddRange(result);
    await _context.SaveChangesAsync();
    Console.WriteLine("Get story success.");
    return  ;
}

async Task GetChapter(int novelId, string novelLink)
{
    var chapters = new List<Chapter>();
    
    // get html chapters
     var crawlLink = $"{url}{novelLink}";
     var chapterHtml = new WebClient().DownloadString(crawlLink);
     var chapterDoc = new HtmlDocument();
     chapterDoc.LoadHtml(chapterHtml);
     
     // get chapters 
     var novelNodes = chapterDoc.DocumentNode.SelectNodes("//ul[@class='khungul']/li");
     if (novelNodes == null)
         return;
     var currentChapters = await _context.Chapters.Where(chapter => chapter.StoryId == novelId).ToListAsync();
     var index = 1;
     foreach (var novelNode in novelNodes??null)
     {
         if(novelNode==null)
             continue;
         // get chapter info
         var chapter = new Chapter();
         var titleNode = novelNode.SelectSingleNode("./*[@class='a1']");
        
         if(titleNode!=null )
         {
             chapter.StoryId = novelId;
             chapter.Title = titleNode.InnerText?.Trim()??"blank";
             if( currentChapters.Any(chap => string.Equals(chap.Title,chapter.Title)))
                 continue;
             chapter.Link = titleNode.GetAttributeValue("href", "");

             // get chapter data
             var dataUrl = $"{url}{chapter.Link}";
             var dataHtml = new WebClient().DownloadString(dataUrl);
             
             //init the html doc
             var dataDoc = new HtmlDocument();
             dataDoc.LoadHtml(dataHtml);
             var dataNode = dataDoc.DocumentNode.SelectSingleNode("//p[@class='truyen'][1]"); 
             var data = dataNode?.InnerText?.Trim();
             chapter.Data = data ?? "blank";
             
             var dateNode = dataDoc.DocumentNode.SelectSingleNode("//p[@class='phai']");
             try
             {

                 var stringDate = dateNode.InnerText?.Trim();
                 var matched = Regex.Match(stringDate, @"\d{2}-\d{2}-\d{4}");
                 if (matched.Success && DateTime.TryParseExact(matched.Groups[0].Value, "dd-MM-yyyy", null, 0, out var date))
                 {
                     chapter.CreatedDate = date;
                 }
                 else
                 {
                     chapter.CreatedDate = DateTime.Now;
                 }
             }
             catch (Exception e)
             {
                 Console.WriteLine("Cannot convert date upload");
                 chapter.CreatedDate = null;
             }

             
             // get description
             var novelContent = dataDoc.DocumentNode.SelectSingleNode("//meta[@name='description']");
             var content = novelContent?.GetAttributeValue("content","blank");
             chapter.Content = content??"blank";
             chapter.Index = index;
             chapters.Add(chapter);
             index++;
         }
     }
     _context.Chapters.AddRange(chapters);
     
     await _context.SaveChangesAsync();
     Console.WriteLine("Get chapter success.");
}