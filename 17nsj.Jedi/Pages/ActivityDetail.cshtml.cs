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
    public class ActivityDetailModel : PageModelBase
    {
        public ActivityDetailModel(JediDbContext dbContext)
            : base(dbContext)
        {

        }

        public ActivityModel CurrentAct { get; private set; }

        public async Task<IActionResult> OnGetAsync(string category, int? id)
        {
            if (category == null || id == null) return new RedirectResult("/NotFound");

            this.PageInitializeAsync();

            var act = await this.DBContext.Activities.Where(x => x.IsAvailable && x.Category == category && x.Id == (int)id).FirstOrDefaultAsync();

            if (act == null) return new RedirectResult("/NotFound");

            var categories = await this.DBContext.ActivityCategories.ToListAsync();
            var currentCategory = categories.Where(x => x.Category == act.Category).FirstOrDefault();

            this.CurrentAct = new ActivityModel();
            CurrentAct.Category = act.Category;
            CurrentAct.CategoryName = currentCategory.CategoryName;
            CurrentAct.CategoryColor = currentCategory.Color;
            CurrentAct.Id = act.Id;
            CurrentAct.ThumbnailURL = act.ThumbnailURL;
            CurrentAct.Title = act.Title;
            CurrentAct.MediaURL = act.MediaURL;
            CurrentAct.Outline = act.Outline;
            CurrentAct.Term = act.Term;
            CurrentAct.Location = act.Location;
            CurrentAct.MapURL = act.MapURL;
            CurrentAct.RelationalURL = act.RelationalURL;
            CurrentAct.CanWaitable = act.CanWaitable;
            CurrentAct.IsClosed = act.IsClosed;
            CurrentAct.WaitingTime = act.WaitingTime;
            CurrentAct.WaitingInfoUpdatedAt = act.WaitingInfoUpdatedAt;
            CurrentAct.IsAvailable = act.IsAvailable;
            CurrentAct.CreatedAt = act.CreatedAt;
            CurrentAct.CreatedBy = act.CreatedBy;
            CurrentAct.UpdatedAt = act.UpdatedAt;
            CurrentAct.UpdatedBy = act.UpdatedBy;
            CurrentAct.Latitude = act.Latitude;
            CurrentAct.Longitude = act.Longitude;

            return this.Page();
        }
    }
}