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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace _17nsj.Jedi.Pages
{
    [Authorize(Roles = UserRoleDomain.SysAdmin)]
    public class NewsPaperManageModel : PageModelBase
    {
        private ILogger _logger;

        public NewsPaperManageModel(JediDbContext dbContext, ILogger<NewsPaperManageModel> logger)
            : base(dbContext)
        {
            _logger = logger;
        }

        [BindProperty]
        public NewsPaperModel TargetNp { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            this.PageInitializeAsync();
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
                return this.Page();
            }

            // 新規作成
            using (var tran = await this.DBContext.Database.BeginTransactionAsync())
            {
                //最大ID取得
                var maxId = await this.DBContext.Newspapers.MaxAsync(x => x.Id);
                this.TargetNp.Id = maxId + 1;

                var entity = new Newspapers();
                var now = DateTime.UtcNow;
                entity.Id = this.TargetNp.Id;
                entity.Title = this.TargetNp.Title;
                entity.URL = this.TargetNp.URL;
                entity.ThumbnailURL = this.TargetNp.ThumbnailURL;
                entity.CreatedAt = now;
                entity.CreatedBy = this.UserID;
                entity.UpdatedAt = now;
                entity.UpdatedBy = this.UserID;

                try
                {
                    await this.DBContext.Newspapers.AddAsync(entity);
                    await this.DBContext.SaveChangesAsync();
                    tran.Commit();
                    _logger.LogInformation($"【新聞登録】ユーザー：{this.UserID}　対象：{this.TargetNp.Id}");
                    return new RedirectResult($"/NewspaperList");
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    _logger.LogError(ex, $"【新聞削除エラー】ユーザー：{this.UserID}　対象：{this.TargetNp.Id}");
                    this.MsgCategory = MsgCategoryDomain.Error;
                    this.Msg = ex.Message;
                    return this.Page();
                }
            }
        }

        private string Validation()
        {
            //タイトルは1~30文字以内
            if (this.TargetNp.Title == null || this.TargetNp.Title.Length < 0 || this.TargetNp.Title.Length > 30)
            {
                return "タイトルは1~30文字で入力してください。";
            }

            if (string.IsNullOrEmpty(this.TargetNp.ThumbnailURL) || !URLUtil.IsUrl(this.TargetNp.ThumbnailURL) || this.TargetNp.ThumbnailURL.Length < 0 || this.TargetNp.ThumbnailURL.Length > 200)
            {
                return "サムネイルURLは1~200文字の正しいURLの形式で入力してください。";
            }

            if (string.IsNullOrEmpty(this.TargetNp.URL) || !URLUtil.IsUrl(this.TargetNp.URL) || this.TargetNp.URL.Length < 0 || this.TargetNp.URL.Length > 200)
            {
                return "URLは1~200文字の正しいURLの形式で入力してください。";
            }

            return null;
        }
    }
}