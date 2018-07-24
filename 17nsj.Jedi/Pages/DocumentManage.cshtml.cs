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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace _17nsj.Jedi.Pages
{
    [Authorize(Roles = UserRoleDomain.SysAdmin)]
    public class DocumentManageModel : PageModelBase
    {
        private ILogger _logger;

        public DocumentManageModel(JediDbContext dbContext, ILogger<DocumentManageModel> logger)
            : base(dbContext)
        {
            _logger = logger;
        }

        [BindProperty]
        public DocumentModel TargetDoc { get; set; }

        [BindProperty]
        public bool IsEditMode { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            this.PageInitializeAsync();

            if (id != null)
            {
                // 既存更新
                this.IsEditMode = true;

                var doc = await this.DBContext.Documents.Where(x => x.Id == id && x.IsAvailable == true).FirstOrDefaultAsync();
                if (doc == null) return new RedirectResult("/NotFound");

                TargetDoc = new DocumentModel();
                TargetDoc.Id = doc.Id;
                TargetDoc.Title = doc.Title;
                TargetDoc.Outline = doc.Outline;
                TargetDoc.ThumbnailURL = doc.ThumbnailURL;
                TargetDoc.URL = doc.URL;
                TargetDoc.UpdatedAt = doc.UpdatedAt;
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
                    var doc = await this.DBContext.Documents.Where(x => x.Id == this.TargetDoc.Id && x.IsAvailable == true).FirstOrDefaultAsync();
                    if (doc == null)
                    {
                        this.MsgCategory = MsgCategoryDomain.Error;
                        this.Msg = メッセージ.選択対象なし;
                        return this.Page();
                    }

                    // 既更新チェック
                    if (doc.UpdatedAt.TruncMillSecond() != this.TargetDoc.UpdatedAt)
                    {
                        this.MsgCategory = MsgCategoryDomain.Error;
                        this.Msg = メッセージ.既更新;
                        return this.Page();
                    }

                    doc.Title = this.TargetDoc.Title;
                    doc.Outline = this.TargetDoc.Outline;
                    doc.URL = this.TargetDoc.URL;
                    doc.ThumbnailURL = this.TargetDoc.ThumbnailURL;
                    doc.UpdatedAt = DateTime.UtcNow;
                    doc.UpdatedBy = this.UserID;

                    try
                    {
                        await this.DBContext.SaveChangesAsync();
                        tran.Commit();
                        _logger.LogInformation($"【資料更新】ユーザー：{this.UserID}　対象：{this.TargetDoc.Id}");
                        return new RedirectResult($"/DocumentList");
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        _logger.LogError(ex, $"【資料更新エラー】ユーザー：{this.UserID}　対象：{this.TargetDoc.Id}");
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
                    return this.Page();
                }

                // 新規作成
                using (var tran = await this.DBContext.Database.BeginTransactionAsync())
                {
                    //最大ID取得
                    var maxId = await this.DBContext.Documents.MaxAsync(x => x.Id);
                    this.TargetDoc.Id = maxId + 1;

                    var entity = new Documents();
                    var now = DateTime.UtcNow;
                    entity.Id = this.TargetDoc.Id;
                    entity.Title = this.TargetDoc.Title;
                    entity.Outline = this.TargetDoc.Outline;
                    entity.URL = this.TargetDoc.URL;
                    entity.ThumbnailURL = this.TargetDoc.ThumbnailURL;
                    entity.IsAvailable = true;
                    entity.CreatedAt = now;
                    entity.CreatedBy = this.UserID;
                    entity.UpdatedAt = now;
                    entity.UpdatedBy = this.UserID;

                    try
                    {
                        await this.DBContext.Documents.AddAsync(entity);
                        await this.DBContext.SaveChangesAsync();
                        tran.Commit();
                        _logger.LogInformation($"【資料登録】ユーザー：{this.UserID}　対象：{this.TargetDoc.Id}");
                        return new RedirectResult($"/DocumentList");
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        _logger.LogError(ex, $"【資料登録エラー】ユーザー：{this.UserID}　対象：{this.TargetDoc.Id}");
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
            if (this.TargetDoc.Title == null || this.TargetDoc.Title.Length <= 0 || this.TargetDoc.Title.Length >= 30)
            {
                return "タイトルは1~30文字で入力してください。";
            }

            if (this.TargetDoc.ThumbnailURL == null || !URLUtil.IsUrl(this.TargetDoc.ThumbnailURL) || this.TargetDoc.ThumbnailURL.Length <= 0 || this.TargetDoc.ThumbnailURL.Length >= 100)
            {
                return "サムネイルURLは1～200文字の正しいURLの形式で入力してください。";
            }

            if (this.TargetDoc.URL == null || !URLUtil.IsUrl(this.TargetDoc.URL) || this.TargetDoc.URL.Length <= 0 || this.TargetDoc.URL.Length >= 100)
            {
                return "URLは1～200文字の正しいURLの形式で入力してください。";
            }

            if (this.TargetDoc.Outline == null || this.TargetDoc.Outline.Length <= 0 || this.TargetDoc.Outline.Length >= 100)
            {
                return "説明は1~100文字で入力してください。";
            }

            return null;
        }
    }
}