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
    [Authorize(Roles = UserRoleDomain.SysAdmin)]
    public class DocumentDeleteModel : PageModelBase
    {
        public DocumentDeleteModel(JediDbContext dbContext)
            : base(dbContext)
        {

        }
        [BindProperty]
        public DocumentModel CurrentDoc { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return new NotFoundResult();

            this.PageInitializeAsync();

            var doc = await this.DBContext.Documents.Where(x => x.Id == (int)id && x.IsAvailable == true).FirstOrDefaultAsync();

            if (doc == null) return new NotFoundResult();

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
                if (CurrentDoc.Id == 0) return new NotFoundResult();

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
                    return new RedirectResult("/DocumentList");
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