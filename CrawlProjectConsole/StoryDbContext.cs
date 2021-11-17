using System;
using Microsoft.EntityFrameworkCore;

namespace CrawlProjectConsole
{
   
    public class StoryDbContext : DbContext
    {
        public DbSet<Story> Stories { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Story>().ToTable("Story");
            modelBuilder.Entity<Chapter>().ToTable("Chapter");
            // modelBuilder.Entity<Story>().Property(m => m.Author).IsRequired(false);  
            // modelBuilder.Entity<Story>().Property(m => m.TotalView).IsRequired(false);  
            // modelBuilder.Entity<Story>().Property(m => m.TotalChapter).IsRequired(false);  
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        { 
            optionsBuilder.UseSqlServer(@"Server=.;Database=CrawlStoryDb;Trusted_Connection=True;");
        }
    }
}