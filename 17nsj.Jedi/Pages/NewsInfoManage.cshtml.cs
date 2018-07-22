using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
    [Authorize(Roles = UserRoleDomain.Writer + "," + UserRoleDomain.Admin + "," + UserRoleDomain.SysAdmin)]
    public class NewsInfoManageModel : PageModelBase
    {
        public NewsInfoManageModel(JediDbContext dbContext)
            : base(dbContext)
        {

        }
        public List<SelectListItem> CategoryList { get; set; }

        [BindProperty]
        public NewsModel TargetNews { get; set; }

        [BindProperty]
        public bool IsEditMode { get; set; }

        [BindProperty]
        public bool IsAuthorized { get; set; }

        public async Task<IActionResult> OnGetAsync(string category, int? id)
        {
            this.PageInitializeAsync();

            if (category != null && id != null)
            {
                // 既存更新
                this.IsEditMode = true;

                var news = await this.DBContext.News.Where(x => x.Category == category && x.Id == id).FirstOrDefaultAsync();
                if (news == null)
                {
                    //対象なしエラー
                    this.MsgCategory = MsgCategoryDomain.Error;
                    this.Msg = メッセージ.選択対象なし;
                    return this.Page();
                }

                //アカウントが管理者ではなく、ニュースが非表示設定になっていたら追い返す
                if (!this.IsAdmin && !news.IsAvailable) return new ForbidResult();

                TargetNews = new NewsModel();
                TargetNews.Category = news.Category;
                TargetNews.Id = news.Id;
                TargetNews.Author = news.Author;
                TargetNews.Title = news.Title;
                TargetNews.Outline = news.Outline;
                TargetNews.MediaURL = news.MediaURL;
                TargetNews.RelationalURL = news.RelationalURL;
                TargetNews.ThumbnailURL = news.ThumbnailURL;
                TargetNews.UpdatedAt = news.UpdatedAt;
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
                    var news = await this.DBContext.News.Where(x => x.Category == this.TargetNews.Category && x.Id == this.TargetNews.Id).FirstOrDefaultAsync();
                    if (news == null)
                    {
                        this.MsgCategory = MsgCategoryDomain.Error;
                        this.Msg = メッセージ.選択対象なし;
                        return this.Page();
                    }

                    // 既更新チェック
                    if (news.UpdatedAt.TruncMillSecond() != this.TargetNews.UpdatedAt)
                    {
                        this.MsgCategory = MsgCategoryDomain.Error;
                        this.Msg = メッセージ.既更新;
                        return this.Page();
                    }

                    news.Author = this.TargetNews.Author;
                    news.Title = this.TargetNews.Title;
                    news.Outline = this.TargetNews.Outline;
                    news.MediaURL = this.TargetNews.MediaURL;
                    news.RelationalURL = this.TargetNews.RelationalURL;
                    news.ThumbnailURL = this.TargetNews.ThumbnailURL;
                    news.UpdatedAt = DateTime.UtcNow;
                    news.UpdatedBy = this.UserID;

                    try
                    {
                        await this.DBContext.SaveChangesAsync();
                        tran.Commit();
                        return new RedirectResult($"/NewsInfoDetail?category={this.TargetNews.Category}&id={this.TargetNews.Id}");
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
                    var maxId = await this.DBContext.News.Where(x => x.Category == this.TargetNews.Category).OrderByDescending(x => x.Id).Select(x => x.Id).FirstOrDefaultAsync();
                    this.TargetNews.Id = maxId + 1;

                    var entity = new News();
                    var now = DateTime.UtcNow;
                    entity.Category = this.TargetNews.Category;
                    entity.Id = this.TargetNews.Id;
                    entity.Author = this.TargetNews.Author;
                    entity.Title = this.TargetNews.Title;
                    entity.Outline = this.TargetNews.Outline;
                    entity.MediaURL = this.TargetNews.MediaURL;
                    entity.RelationalURL = this.TargetNews.RelationalURL;
                    entity.ThumbnailURL = this.TargetNews.ThumbnailURL;
                    entity.IsAvailable = true;
                    entity.CreatedAt = now;
                    entity.CreatedBy = this.UserID;
                    entity.UpdatedAt = now;
                    entity.UpdatedBy = this.UserID;

                    try
                    {
                        await this.DBContext.News.AddAsync(entity);
                        await this.DBContext.SaveChangesAsync();
                        tran.Commit();
                        return new RedirectResult($"/NewsInfoDetail?category={this.TargetNews.Category}&id={this.TargetNews.Id}");
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
            if (this.TargetNews.Title == null || this.TargetNews.Title.Length <= 0 || this.TargetNews.Title.Length >= 30)
            {
                return "タイトルは1~30文字で入力してください。";
            }

            //取材担当は1~30文字以内
            if (this.TargetNews.Author == null || this.TargetNews.Author.Length <= 0 || this.TargetNews.Author.Length >= 30)
            {
                return "取材担当は1~30文字で入力してください。";
            }

            if(!string.IsNullOrEmpty(this.TargetNews.ThumbnailURL) && !URLUtil.IsUrl(this.TargetNews.ThumbnailURL))
            {
                return "サムネイルURLは正しいURLの形式で入力してください。";
            }

            if (!string.IsNullOrEmpty(this.TargetNews.MediaURL) && !URLUtil.IsUrl(this.TargetNews.MediaURL))
            {
                return "画像URLは正しいURLの形式で入力してください。";
            }

            if (!string.IsNullOrEmpty(this.TargetNews.RelationalURL) && !URLUtil.IsUrl(this.TargetNews.RelationalURL))
            {
                return "関連URLは正しいURLの形式で入力してください。";
            }

            if (!this.IsAuthorized)
            {
                return "本記事が承認されたものであるかを確認してください。";
            }

            return null;
        }

        public async Task GetCategorySelectListItemsAsync()
        {
            this.CategoryList = new List<SelectListItem>();

            var list = await this.DBContext.NewsCategories.ToListAsync();

            foreach (var item in list)
            {
                this.CategoryList.Add(new SelectListItem() { Text = item.CategoryName, Value = item.Category });
            }
        }
    }
}