using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _17nsj.DataAccess;
using _17nsj.Jedi.Constants;
using _17nsj.Jedi.Domains;
using _17nsj.Jedi.Extensions;
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

        [BindProperty]
        public bool IsEditMode { get; set; }

        public async Task<IActionResult> OnGetAsync(string category, int? id)
        {
            if (category != null && id != null)
            {
                // 既存更新
                this.IsEditMode = true;

                var act = await this.DBContext.Activities.Where(x => x.Category == category && x.Id == id).FirstOrDefaultAsync();
                if (act == null) return new RedirectResult("/NotFound");

                TargetAct = new ActivityModel();
                TargetAct.Category = act.Category;
                TargetAct.Id = act.Id;
                TargetAct.Title = act.Title;
                TargetAct.Outline = act.Outline;
                TargetAct.MediaURL = act.MediaURL;
                TargetAct.RelationalURL = act.RelationalURL;
                TargetAct.ThumbnailURL = act.ThumbnailURL;
                TargetAct.Term = act.Term;
                TargetAct.Location = act.Location;
                TargetAct.Latitude = act.Latitude;
                TargetAct.Longitude = act.Longitude;
                TargetAct.UpdatedAt = act.UpdatedAt;
            }
            else
            {
                // 新規作成
                this.IsEditMode = false;
                await GetCategorySelectListItemsAsync();
            }

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            this.PageInitializeAsync();

            if (this.IsEditMode)
            {
                var val = Validation();
                if (val != null)
                {
                    this.MsgCategory = MsgCategoryDomain.Error;
                    this.Msg = val;
                    return this.Page();
                }

                //更新
                using (var tran = await this.DBContext.Database.BeginTransactionAsync())
                {
                    //存在チェック
                    var act = await this.DBContext.Activities.Where(x => x.Category == this.TargetAct.Category && x.Id == this.TargetAct.Id).FirstOrDefaultAsync();
                    if (act == null)
                    {
                        this.MsgCategory = MsgCategoryDomain.Error;
                        this.Msg = メッセージ.選択対象なし;
                        return this.Page();
                    }

                    // 既更新チェック
                    if (act.UpdatedAt.TruncMillSecond() != this.TargetAct.UpdatedAt)
                    {
                        this.MsgCategory = MsgCategoryDomain.Error;
                        this.Msg = メッセージ.既更新;
                        return this.Page();
                    }

                    act.Title = this.TargetAct.Title;
                    act.Outline = this.TargetAct.Outline;
                    act.MediaURL = this.TargetAct.MediaURL;
                    act.RelationalURL = this.TargetAct.RelationalURL;
                    act.ThumbnailURL = this.TargetAct.ThumbnailURL;
                    act.Term = this.TargetAct.Term;
                    act.Location = this.TargetAct.Location;
                    act.Latitude = this.TargetAct.Latitude;
                    act.Longitude = this.TargetAct.Longitude;
                    act.UpdatedAt = DateTime.UtcNow;
                    act.UpdatedBy = this.UserID;

                    try
                    {
                        await this.DBContext.SaveChangesAsync();
                        tran.Commit();
                        return new RedirectResult($"/ActivityList");
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
            else
            {
                var val = Validation();
                if (val != null)
                {
                    this.MsgCategory = MsgCategoryDomain.Error;
                    this.Msg = val;
                    await GetCategorySelectListItemsAsync();
                    return this.Page();
                }

                // 新規作成
                using (var tran = await this.DBContext.Database.BeginTransactionAsync())
                {
                    //最大ID取得
                    var maxId = await this.DBContext.Activities.Where(x => x.Category == this.TargetAct.Category).OrderByDescending(x => x.Id).Select(x => x.Id).FirstOrDefaultAsync();
                    this.TargetAct.Id = maxId + 1;

                    var entity = new Activities();
                    var now = DateTime.UtcNow;
                    entity.Category = this.TargetAct.Category;
                    entity.Id = this.TargetAct.Id;
                    entity.MediaURL = this.TargetAct.MediaURL;
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
        }

        private string Validation()
        {
            //タイトルは1~30文字以内
            if (this.TargetAct.Title == null || this.TargetAct.Title.Length <= 0 || this.TargetAct.Title.Length >= 30)
            {
                return "タイトルは1~30文字で入力してください。";
            }

            if (this.TargetAct.ThumbnailURL != null)
            {
                if ((!string.IsNullOrEmpty(this.TargetAct.ThumbnailURL) && !URLUtil.IsUrl(this.TargetAct.ThumbnailURL)) || (this.TargetAct.ThumbnailURL.Length <= 0 || this.TargetAct.ThumbnailURL.Length >= 200))
                {
                    return "サムネイルURLは正しいURLの形式で200文字以内で入力してください。";
                }
            }

            if (this.TargetAct.MediaURL != null)
            {
                if ((!string.IsNullOrEmpty(this.TargetAct.MediaURL) && !URLUtil.IsUrl(this.TargetAct.MediaURL)) || (this.TargetAct.ThumbnailURL.Length <= 0 || this.TargetAct.ThumbnailURL.Length >= 200))
                {
                    return "画像URLは正しいURLの形式で200文字以内で入力してください。";
                }
            }

            if (this.TargetAct.RelationalURL != null)
            {
                if ((!string.IsNullOrEmpty(this.TargetAct.RelationalURL) && !URLUtil.IsUrl(this.TargetAct.RelationalURL)) || (this.TargetAct.ThumbnailURL.Length <= 0 || this.TargetAct.ThumbnailURL.Length >= 200))
                {
                    return "関連URLは正しいURLの形式で200文字以内で入力してください。";
                }
            }

            if (this.TargetAct.Outline == null || this.TargetAct.Outline.Length <= 0 || this.TargetAct.Outline.Length >= 500)
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