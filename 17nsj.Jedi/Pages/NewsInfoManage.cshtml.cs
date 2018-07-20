using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _17nsj.DataAccess;
using _17nsj.Jedi.Constants;
using _17nsj.Jedi.Domains;
using _17nsj.Jedi.Extensions;
using _17nsj.Jedi.Models;
using _17nsj.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        [BindProperty]
        public NewsModel TargetNews { get; set; }

        [BindProperty]
        public bool IsEditMode { get; set; }

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
            }
            else
            {
                // 新規作成
                this.IsEditMode = false;
            }

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            this.PageInitializeAsync();

            if (this.IsEditMode)
            {
                var val = UpdateValidation();
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
                var val = CreateValidation();
                if (val != null)
                {
                    this.MsgCategory = MsgCategoryDomain.Error;
                    this.Msg = val;
                    return this.Page();
                }

                // 新規作成
                using (var tran = await this.DBContext.Database.BeginTransactionAsync())
                {
                    //最大ID取得
                    var exist = await this.DBContext.News.Where(x => x.Category == this.TargetNews.Category && x.Id == this.TargetNews.Id).AnyAsync();
                    if (exist)
                    {
                        this.MsgCategory = MsgCategoryDomain.Error;
                        this.Msg = メッセージ.ユーザーID重複;
                        return this.Page();
                    }

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

        private string CreateValidation()
        {
            //表示名は1~30文字以内
            if (this.TargetNews.Title == null || this.TargetNews.Title.Length <= 0 || this.TargetNews.Title.Length >= 30)
            {
                return "表示名は1~30文字で入力してください。";
            }

            return null;
        }

        private string UpdateValidation()
        {
            //表示名は1~30文字以内
            if (this.TargetNews.Title == null || this.TargetNews.Title.Length <= 0 || this.TargetNews.Title.Length >= 30)
            {
                return "表示名は1~30文字で入力してください。";
            }

            return null;
        }
    }
}