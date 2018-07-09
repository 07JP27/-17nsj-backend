using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _17nsj.Jedi.Models;
using _17nsj.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace _17nsj.Jedi.Pages
{
    public class NewsInfoDetailModel : PageModelBase
    {
        public NewsInfoDetailModel(JediDbContext dbContext)
            : base(dbContext)
        {

        }

        public NewsModel CurrentNews { get; private set; }

        public async Task<IActionResult> OnGetAsync(string category, int? id)
        {
            if(category == null || id == null) return new NotFoundResult();

            this.PageInitializeAsync();

            var news = await this.DBContext.News.Where(x => x.IsAvailable && x.Category == category && x.Id == (int)id).FirstOrDefaultAsync();

            if (news == null) return new NotFoundResult();

            var categories = await this.DBContext.NewsCategories.ToListAsync();
            var currentCategory = categories.Where(x => x.Category == news.Category).FirstOrDefault();

            this.CurrentNews = new NewsModel();
            CurrentNews.Category = news.Category;
            CurrentNews.CategoryStr = currentCategory.CategoryName;
            CurrentNews.CategoryColor = currentCategory.Color;
            CurrentNews.Id = news.Id;
            CurrentNews.Author = news.Author;
            CurrentNews.Title = news.Title;
            CurrentNews.MediaURL = news.MediaURL;
            CurrentNews.Outline = news.Outline;
            CurrentNews.MediaURL = news.MediaURL;
            CurrentNews.RelationalURL = news.RelationalURL;
            CurrentNews.CreatedAt = news.CreatedAt;
            CurrentNews.UpdatedAt = news.UpdatedAt;

            return this.Page();
        }
    }
}