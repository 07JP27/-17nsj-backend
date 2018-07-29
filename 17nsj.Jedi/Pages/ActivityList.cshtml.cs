using System;
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
    public class ActivityListModel : PageModelBase
    {
        public ActivityListModel(JediDbContext dbContext)
            : base(dbContext)
        {

        }

        public List<ActivityModel> プログラムリスト { get; private set; }
        public List<ActivityCategoryModel> カテゴリーリスト { get; private set; }
        public string クエリ { get; private set; }

        public async Task<IActionResult> OnGetAsync(string q)
        {
            this.PageInitializeAsync();
            var categories = await this.DBContext.ActivityCategories.ToListAsync();

            IQueryable<Activities> actQuery;

            if (string.IsNullOrEmpty(q))
            {
                actQuery = this.DBContext.Activities.Where(x => x.IsAvailable == true).OrderByDescending(x => x.CreatedAt);
            }
            else
            {
                this.クエリ = q;
                actQuery = this.DBContext.Activities.Where(x => x.IsAvailable == true).OrderByDescending(x => x.CreatedAt).Where(x => x.Title.Contains(q) || x.Outline.Contains(q) || x.Location.Contains(q));
            }

            カテゴリーリスト = new List<ActivityCategoryModel>();
            foreach (var item in categories)
            {
                var model = new ActivityCategoryModel();
                model.Category = item.Category;
                model.CategoryName = item.CategoryName;
                model.Color = item.Color;
                model.ThumbnailURL = item.ThumbnailURL;
                カテゴリーリスト.Add(model);
            }

            プログラムリスト = new List<ActivityModel>();
            foreach (var item in await actQuery.Select(x => new { x.Category, x.Id, x.ThumbnailURL, x.Title, x.Term, x.Location, x.UpdatedAt }).ToListAsync())
            {
                var model = new ActivityModel();
                model.Category = item.Category;
                model.Id = item.Id;
                model.CategoryColor = categories.Where(x => x.Category == item.Category).FirstOrDefault().Color;

                if (string.IsNullOrEmpty(item.ThumbnailURL))
                {
                    model.ThumbnailURL = categories.Where(x => x.Category == item.Category).FirstOrDefault().ThumbnailURL;
                }
                else
                {
                    model.ThumbnailURL = item.ThumbnailURL;
                }

                model.Title = item.Title;
                model.Term = item.Term;
                model.Location = item.Location;
                プログラムリスト.Add(model);
            }

            return this.Page();
        }
    }
}