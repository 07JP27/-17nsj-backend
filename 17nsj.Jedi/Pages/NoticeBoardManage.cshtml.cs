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
using _17nsj.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace _17nsj.Jedi.Pages
{
    [Authorize(Roles=UserRoleDomain.Admin + "," + UserRoleDomain.SysAdmin)]
    public class NoticeBoardManageModel : PageModelBase
    {
        private ILogger _logger;

        public NoticeBoardManageModel(JediDbContext dbContext, ILogger<NoticeBoardManageModel> logger)
            : base(dbContext)
        {
            _logger = logger;
        }

        [BindProperty]
        public NoticeModel TargetNotice { get; set; }

        [BindProperty]
        public bool IsEditMode { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            this.PageInitializeAsync();

            if (id != null)
            {
                // 既存更新
                this.IsEditMode = true;

                var notice = await this.DBContext.NoticeBoard.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (notice == null) return new RedirectResult("/NotFound");

                if (!this.IsSysAdmin && notice.CreatedBy != this.UserID) return new ForbidResult();

                TargetNotice = new NoticeModel();
                var now = DateTime.UtcNow;
                TargetNotice.Id = notice.Id;
                TargetNotice.Title = notice.Title;
                TargetNotice.Contents = notice.Contents;
                TargetNotice.Receiver = notice.Receiver;
                TargetNotice.Sender = notice.Sender;
                TargetNotice.TerminationStr = notice.Termination.AddHours(9).ToString("yyyy/MM/dd HH:mm");
                TargetNotice.CreatedBy = notice.CreatedBy;
                TargetNotice.UpdatedAt = notice.UpdatedAt;
            }
            else
            {
                // 新規作成
                this.IsEditMode = false;
                this.TargetNotice = new NoticeModel();
                this.TargetNotice.Sender = this.UserName;
                this.TargetNotice.TerminationStr = DateTime.UtcNow.AddHours(9).AddDays(1).ToString("yyyy/MM/dd HH:mm");
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
                    var notice = await this.DBContext.NoticeBoard.Where(x => x.Id == this.TargetNotice.Id).FirstOrDefaultAsync();
                    if (notice == null)
                    {
                        this.MsgCategory = MsgCategoryDomain.Error;
                        this.Msg = メッセージ.選択対象なし;
                        return this.Page();
                    }

                    if (!this.IsSysAdmin && notice.CreatedBy != this.UserID) return new ForbidResult();

                    // 既更新チェック
                    if (notice.UpdatedAt.TruncMillSecond() != TargetNotice.UpdatedAt)
                    {
                        this.MsgCategory = MsgCategoryDomain.Error;
                        this.Msg = メッセージ.既更新;
                        return this.Page();
                    }

                    notice.Title = this.TargetNotice.Title;
                    notice.Contents = this.TargetNotice.Contents;
                    notice.Receiver = this.TargetNotice.Receiver;
                    notice.Sender = this.TargetNotice.Sender;
                    notice.Termination = DateTime.Parse(this.TargetNotice.TerminationStr).AddHours(-9);
                    notice.UpdatedAt = DateTime.UtcNow;
                    notice.UpdatedBy = this.UserID;

                    try
                    {
                        await this.DBContext.SaveChangesAsync();
                        tran.Commit();
                        _logger.LogInformation($"【掲示更新】ユーザー：{this.UserID}　対象：{this.TargetNotice.Id}");
                        return new RedirectResult("/NoticeBoardList");
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        _logger.LogError(ex, $"【掲示更新エラー】ユーザー：{this.UserID}　対象：{this.TargetNotice.Id}");
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
                    //存在チェック
                    var maxId = await this.DBContext.NoticeBoard.MaxAsync(x => x.Id);

                    var entity = new NoticeBoard();
                    var now = DateTime.UtcNow;
                    entity.Id = maxId + 1;
                    entity.Title = this.TargetNotice.Title;
                    entity.Contents = this.TargetNotice.Contents;
                    entity.Receiver = this.TargetNotice.Receiver;
                    entity.Sender = this.TargetNotice.Sender;
                    entity.Termination = DateTime.Parse(this.TargetNotice.TerminationStr).AddHours(-9);
                    entity.CreatedAt = now;
                    entity.CreatedBy = this.UserID;
                    entity.UpdatedAt = now;
                    entity.UpdatedBy = this.UserID;

                    try
                    {
                        await this.DBContext.NoticeBoard.AddAsync(entity);
                        await this.DBContext.SaveChangesAsync();
                        tran.Commit();
                        _logger.LogInformation($"【掲示登録】ユーザー：{this.UserID}　対象：{this.TargetNotice.Id}");
                        return new RedirectResult("/NoticeBoardList");
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        _logger.LogError(ex, $"【掲示登録エラー】ユーザー：{this.UserID}　対象：{this.TargetNotice.Id}");
                        this.MsgCategory = MsgCategoryDomain.Error;
                        this.Msg = ex.Message;
                        return this.Page();
                    }
                }
            }
        }

        private string Validation()
        {
            //タイトルは半角1~30文字
            if (this.TargetNotice.Title == null || this.TargetNotice.Title.Length < 0 || this.TargetNotice.Title.Length > 30)
            {
                return "タイトルは1~30文字で入力してください。";
            }

            //内容は半角1~1000文字
            if (this.TargetNotice.Contents == null || this.TargetNotice.Contents.Length < 0 || this.TargetNotice.Contents.Length > 1000)
            {
                return "内容は1~1000文字で入力してください。";
            }

            //宛先は半角1~30文字
            if (this.TargetNotice.Receiver == null || this.TargetNotice.Receiver.Length < 0 || this.TargetNotice.Receiver.Length > 30)
            {
                return "宛先は1~30文字で入力してください。";
            }

            //発信者は半角1~30文字
            if (this.TargetNotice.Sender == null || this.TargetNotice.Sender.Length < 0 || this.TargetNotice.Sender.Length > 30)
            {
                return "発信者は1~30文字で入力してください。";
            }

            //掲載期限は必須
            if (string.IsNullOrEmpty(this.TargetNotice.TerminationStr))
            {
                return "掲載期限を入力してください。";
            }

            return null;
        }
    }
}