﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _17nsj.DataAccess;
using _17nsj.Jedi.Models;
using _17nsj.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace _17nsj.Jedi.Pages
{
    public class NewsInfoListModel : PageModelBase
    {
        public NewsInfoListModel(JediDbContext dbContext)
            : base(dbContext)
        {

        }

        public List<NewsModel> ニュースリスト { get; private set; }
        public List<NewsCategoryModel> カテゴリーリスト { get; private set; }
        public string クエリ { get; private set; }

        public async Task<IActionResult> OnGetAsync(string q)
        {
            this.PageInitializeAsync();
            var categories = await this.DBContext.NewsCategories.ToListAsync();
            IQueryable<News> newsQuery;
            
            if(string.IsNullOrEmpty(q))
            {
                newsQuery = this.DBContext.News.OrderByDescending(x => x.UpdatedAt);
            }
            else
            {
                this.クエリ = q;
                newsQuery = this.DBContext.News.OrderByDescending(x => x.UpdatedAt).Where(x => x.Title.Contains(q) || x.Outline.Contains(q));
            }

            カテゴリーリスト = new List<NewsCategoryModel>();
            foreach(var item in categories)
            {
                var model = new NewsCategoryModel();
                model.Category = item.Category;
                model.CategoryName = item.CategoryName;
                model.Color = item.Color;
                model.ThumbnailURL = item.ThumbnailURL;
                カテゴリーリスト.Add(model);
            }

            ニュースリスト = new List<NewsModel>();
            foreach(var item in await newsQuery.Select(x => new { x.Category, x.Id, x.ThumbnailURL, x.Title, x.Outline, x.IsAvailable, x.CreatedBy, x.UpdatedAt }).ToListAsync())
            {
                if (!this.IsAdmin && !item.IsAvailable) continue;
                var model = new NewsModel();
                model.Category = item.Category;
                model.Id = item.Id;
                model.CategoryColor = categories.Where(x => x.Category == item.Category).FirstOrDefault().Color;
                
                if(string.IsNullOrEmpty(item.ThumbnailURL))
                {
                    model.ThumbnailURL = categories.Where(x => x.Category == item.Category).FirstOrDefault().ThumbnailURL;
                }
                else
                {
                    model.ThumbnailURL = item.ThumbnailURL;
                }

                model.Title = item.Title;
                model.Outline = item.Outline;
                model.IsAvailable = item.IsAvailable;
                model.CreatedBy = item.CreatedBy;
                ニュースリスト.Add(model);
            }

            return this.Page();
        }
    }
}