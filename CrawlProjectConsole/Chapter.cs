using System;

namespace CrawlProjectConsole
{
    public class Chapter
    {
        public Chapter()
        {
            Index = 0;
            Title = "";
            Content = "";
            TotalView = 0;
            Link = "";
            CreatedDate = null;
        }

        public int Id { get; set; }
        public int Index { get; set; }
        public string Title { get; set; }
        public string Content { get; set; } // desctiption
        public string Data { get; set; }
        public int TotalView { get; set; }
        public string Link { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int StoryId { get; set; } 
        public Story Story { get; set; }

    }
}