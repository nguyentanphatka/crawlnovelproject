// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using CrawlProjectConsole;
using CrawlProjectConsole.Migrations;
using HtmlAgilityPack;


// // From Web
// var url = "https://www.kiemhieptruyen.com/muc-luc/";
// var web = new HtmlWeb();
// web.OverrideEncoding = Encoding.UTF8;
// var doc = web.Load(url);
//
// // foreach (HtmlNode row in doc.DocumentNode.SelectNodes("//a[@class='a1']")) 
// var data = doc.DocumentNode.SelectNodes("menu-ngang");

// download html
var url = @"https://www.kiemhieptruyen.com/";
var html = new WebClient().DownloadString(url);

//init the html doc
var doc = new HtmlDocument();
doc.LoadHtml(html);
var context = new StoryDbContext();
//test xpath
var novelNodes = doc.DocumentNode.SelectNodes("//ul[@class='khungul']/li");
foreach (var novelNode in novelNodes)
{
    var novel = new Story();
    var titleNode = novelNode.SelectSingleNode("./*[@class='a1']");
    novel.Title = titleNode.InnerText?.Trim();
    novel.Link = titleNode.GetAttributeValue("href", "");
    
    var dateNode = novelNode.SelectSingleNode("./*[@class='a2']");
    var stringDate = dateNode.InnerText?.Trim();
    // Ngày đăng:  28-03-2017 
    // TODO using regex to get date

    GetChapter(novel, novel.Link);
    novel.TotalChapter = novel.Chapters.Count;
    // novel.Chapters = chapters.ToList();
    context.Stories.Add(novel);
    context.SaveChanges();
    Console.WriteLine(stringDate);
}
// context.SaveChanges();

Console.ReadKey();

void GetChapter(Story novel, string href)
 {
     var result = new List<Chapter>();
     // get chapters
     var crawlLink = $"{url}{href}";
     var chapterHtml = new WebClient().DownloadString(crawlLink);
     var chapterDoc = new HtmlDocument();
     chapterDoc.LoadHtml(chapterHtml);
     
     
     // get author
     var novelAuthor = doc.DocumentNode.SelectSingleNode("//meta[@name='author']");
     var author = novelAuthor?.GetAttributeValue("content","blank");
     novel.Author = author;

     // Get total chap from xpath or count
     
     // get chapters
     var novelNodes = chapterDoc.DocumentNode.SelectNodes("//ul[@class='khungul']/li");
     
     var index = 1;
     foreach (var novelNode in novelNodes)
     {
         if(novelNode==null)
             continue;
         // get chapter info
         var chapter = new Chapter();
         var titleNode = novelNode.SelectSingleNode("./*[@class='a1']");
        
         if(titleNode!=null)
         {
             chapter.Title = titleNode.InnerText?.Trim();
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
             // Ngày đăng:  28-03-2017 
             // TODO using regex to get date
             // TODO get index from title of chapter - using regex - update current code
             
             // get description
             var novelContent = dataDoc.DocumentNode.SelectSingleNode("//meta[@name='description']");
             var content = novelContent?.GetAttributeValue("content","blank");
             chapter.Content = content;
             chapter.Index = index;
             result.Add(chapter);
             index++;
         }
     }
     novel.Chapters.AddRange(result);
 }