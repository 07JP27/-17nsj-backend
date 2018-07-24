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
using Microsoft.Extensions.Logging;

namespace _17nsj.Jedi.Pages
{
    [Authorize(Roles = UserRoleDomain.SysAdmin)]
    public class DocumentDeleteModel : PageModelBase
    {
        private ILogger _logger;

        public DocumentDeleteModel(JediDbContext dbContext, ILogger<DocumentDeleteModel> logger)
            : base(dbContext)
        {
            _logger = logger;
        }
        [BindProperty]
        public DocumentModel CurrentDoc { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return new RedirectResult("/NotFound");

            this.PageInitializeAsync();

            var doc = await this.DBContext.Documents.Where(x => x.Id == (int)id && x.IsAvailable == true).FirstOrDefaultAsync();

            if (doc == null) return new RedirectResult("/NotFound");

            this.CurrentDoc = new DocumentModel();
            CurrentDoc.Id = doc.Id;
            CurrentDoc.Title = doc.Title;
            CurrentDoc.UpdatedAt = doc.UpdatedAt;

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            using (var tran = await this.DBContext.Database.BeginTransactionAsync())
            {
                if (CurrentDoc.Id == 0) return new RedirectResult("/NotFound");

                this.PageInitializeAsync();

                var doc = await this.DBContext.Documents.Where(x => x.Id == CurrentDoc.Id && x.IsAvailable == true).FirstOrDefaultAsync();
                if (doc == null)
                {
                    //対象なしエラー
                    this.MsgCategory = MsgCategoryDomain.Error;
                    this.Msg = メッセージ.選択対象なし;
                    return this.Page();
                }

                // 既更新チェック
                if (doc.UpdatedAt.TruncMillSecond() != CurrentDoc.UpdatedAt)
                {
                    this.MsgCategory = MsgCategoryDomain.Error;
                    this.Msg = メッセージ.既更新;
                    return this.Page();
                }

                doc.IsAvailable = false;
                doc.UpdatedAt = DateTime.UtcNow;
                doc.UpdatedBy = this.UserID;

                try
                {
                    await this.DBContext.SaveChangesAsync();
                    tran.Commit();
                    _logger.LogInformation($"【資料削除】ユーザー：{this.UserID}　対象：{this.CurrentDoc.Id}");
                    return new RedirectResult("/DocumentList");
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    _logger.LogError(ex, $"【資料削除エラー】ユーザー：{this.UserID}　対象：{this.CurrentDoc.Id}");
                    this.MsgCategory = MsgCategoryDomain.Error;
                    this.Msg = ex.Message;
                    return this.Page();
                }
            }
        }
    }
}