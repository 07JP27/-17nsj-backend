using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _17nsj.DataAccess;
using _17nsj.Jedi.Domains;
using _17nsj.Jedi.Models;
using _17nsj.Jedi.Utils;
using _17nsj.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace _17nsj.Jedi.Pages
{
    [Authorize(Roles = UserRoleDomain.SysAdmin)]
    public class ActivityManageModel : PageModelBase
    {
        public ActivityManageModel(JediDbContext dbContext)
            : base(dbContext)
        {

        }
        public List<SelectListItem> CategoryList { get; set; }

        [BindProperty]
        public ActivityModel TargetAct { get; set; }

        public async Task<IActionResult> OnGetAsync(string category, int? id)
        {
            this.PageInitializeAsync();
            await GetCategorySelectListItemsAsync();
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            this.PageInitializeAsync();

            var val = Validation();
            if (val != null)
            {
                this.MsgCategory = MsgCategoryDomain.Error;
                this.Msg = val;
                await GetCategorySelectListItemsAsync();
                return this.Page();
            }

            using (var tran = await this.DBContext.Database.BeginTransactionAsync())
            {
                //最大ID取得
                var maxId = await this.DBContext.Activities.Where(x => x.Category == this.TargetAct.Category).OrderByDescending(x => x.Id).Select(x => x.Id).FirstOrDefaultAsync();
                this.TargetAct.Id = maxId + 1;

                var entity = new Activities();
                var now = DateTime.UtcNow;
                entity.Category = this.TargetAct.Category;
                entity.Id = this.TargetAct.Id;
                entity.ThumbnailURL = this.TargetAct.ThumbnailURL;
                entity.Title = this.TargetAct.Title;
                entity.Outline = this.TargetAct.Outline;
                entity.Term = this.TargetAct.Term;
                entity.Location = this.TargetAct.Location;
                entity.RelationalURL = this.TargetAct.RelationalURL;
                entity.Latitude = this.TargetAct.Latitude;
                entity.Longitude = this.TargetAct.Longitude;
                entity.IsAvailable = true;
                entity.CreatedAt = now;
                entity.CreatedBy = this.UserID;
                entity.UpdatedAt = now;
                entity.UpdatedBy = this.UserID;

                try
                {
                    await this.DBContext.Activities.AddAsync(entity);
                    await this.DBContext.SaveChangesAsync();
                    tran.Commit();
                    return new RedirectResult("/ActivityList");
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    this.MsgCategory = MsgCategoryDomain.Error;
                    this.Msg = ex.Message;
                    return this.Page();
                }
            }            
        }

        private string Validation()
        {
            //タイトルは1~30文字以内
            if (this.TargetAct.Title == null || this.TargetAct.Title.Length <= 0 || this.TargetAct.Title.Length >= 30)
            {
                return "タイトルは1~30文字で入力してください。";
            }

            if ((!string.IsNullOrEmpty(this.TargetAct.ThumbnailURL) && !URLUtil.IsUrl(this.TargetAct.ThumbnailURL)) || (this.TargetAct.ThumbnailURL.Length <= 0 || this.TargetAct.ThumbnailURL.Length >= 200))
            {
                return "サムネイルURLは正しいURLの形式で200文字以内で入力してください。";
            }

            if ((!string.IsNullOrEmpty(this.TargetAct.MediaURL) && !URLUtil.IsUrl(this.TargetAct.MediaURL)) || (this.TargetAct.ThumbnailURL.Length <= 0 || this.TargetAct.ThumbnailURL.Length >= 200))
            {
                return "画像URLは正しいURLの形式で200文字以内で入力してください。";
            }

            if ((!string.IsNullOrEmpty(this.TargetAct.RelationalURL) && !URLUtil.IsUrl(this.TargetAct.RelationalURL)) || (this.TargetAct.ThumbnailURL.Length <= 0 || this.TargetAct.ThumbnailURL.Length >= 200))
            {
                return "関連URLは正しいURLの形式で200文字以内で入力してください。";
            }

            if (this.TargetAct.ThumbnailURL.Length <= 0 || this.TargetAct.ThumbnailURL.Length >= 500)
            {
                return "説明は500文字以内で入力してください。";
            }

            return null;
        }

        public async Task GetCategorySelectListItemsAsync()
        {
            this.CategoryList = new List<SelectListItem>();

            var list = await this.DBContext.ActivityCategories.ToListAsync();

            foreach (var item in list)
            {
                this.CategoryList.Add(new SelectListItem() { Text = item.CategoryName, Value = item.Category });
            }
        }
    }
}