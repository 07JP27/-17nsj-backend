using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    [Authorize(Roles = UserRoleDomain.Admin + "," + UserRoleDomain.SysAdmin)]
    public class NewsInfoSwitchEnableModel : PageModelBase
    {
        public NewsInfoSwitchEnableModel(JediDbContext dbContext)
            : base(dbContext)
        {

        }

        [BindProperty]
        public NewsModel CurrentNews { get; set; }

        public async Task<IActionResult> OnGetAsync(string category, int? id)
        {
            if (category == null || id == null) return new RedirectResult("/NotFound");

            this.PageInitializeAsync();

            var news = await this.DBContext.News.Where(x => x.Category == category && x.Id == (int)id).FirstOrDefaultAsync();

            if (news == null) return new RedirectResult("/NotFound");

            this.CurrentNews = new NewsModel();
            CurrentNews.Category = news.Category;
            CurrentNews.Id = news.Id;
            CurrentNews.IsAvailable = news.IsAvailable;
            CurrentNews.UpdatedAt = news.UpdatedAt;

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            using (var tran = await this.DBContext.Database.BeginTransactionAsync())
            {
                if (string.IsNullOrEmpty(CurrentNews.Category) || CurrentNews.Id == 0) return new RedirectResult("/NotFound");

                this.PageInitializeAsync();

                var news = await this.DBContext.News.Where(x => x.Category == CurrentNews.Category && x.Id == CurrentNews.Id).FirstOrDefaultAsync();
                if (news == null)
                {
                    //対象なしエラー
                    this.MsgCategory = MsgCategoryDomain.Error;
                    this.Msg = メッセージ.選択対象なし;
                    return this.Page();
                }

                // 既更新チェック
                if (news.UpdatedAt.TruncMillSecond() != CurrentNews.UpdatedAt)
                {
                    this.MsgCategory = MsgCategoryDomain.Error;
                    this.Msg = メッセージ.既更新;
                    return this.Page();
                }

                if(news.IsAvailable)
                {
                    news.IsAvailable = false;
                }
                else
                {
                    news.IsAvailable = true;
                }

                news.UpdatedAt = DateTime.UtcNow;
                news.UpdatedBy = this.UserID;

                try
                {
                    await this.DBContext.SaveChangesAsync();
                    tran.Commit();
                    return new RedirectResult("/NewsInfoList");
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
}