using System.Collections.Generic;
namespace CrawlProjectConsole
{
    public class Story
    {
        public Story()
        {
            Title = "";
            Author = "";
            Type = "";
            TotalChapter = 0;
            Rate = 0;
            TotalView = 0;
            Link = "";
        }
        // public Story(int id, string title, string author, string type, int totalChapter, float rate, int totalView, string link)
        // {
        //     Id = id;
        //     Title = title;
        //     Author = author;
        //     Type = type;
        //     TotalChapter = totalChapter;
        //     Rate = rate;
        //     TotalView = totalView;
        //     Link = link;
        // }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Type { get; set; }
        public int TotalChapter { get; set; }
        public float Rate { get; set; }
        public int TotalView { get; set; }

        public string Link { get; set; }
        public List<Chapter> Chapters { get; set; } = new List<Chapter>();
    }
}