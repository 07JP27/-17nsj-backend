using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using _17nsj.Jedi.Constants;
using _17nsj.Jedi.Domains;
using _17nsj.Jedi.Extensions;
using _17nsj.Jedi.Models;
using _17nsj.Jedi.Utils;
using _17nsj.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace _17nsj.Jedi.Pages
{
    public class UserProfileModel : PageModelBase
    {
        public UserProfileModel(JediDbContext dbContext)
            : base(dbContext)
        {

        }

        [BindProperty]
        public UserModel TargetUser { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            this.PageInitializeAsync();

            if (string.IsNullOrEmpty(id)) return new NotFoundResult();
            if (id != this.UserID) return new ForbidResult();

            //特殊ユーザーチェック
            if (AppConstants.UndeliteableUsers.Contains(id))
            {
                return new ForbidResult();
            }

            var user = await this.DBContext.Users.Where(x => x.UserId == id && x.IsAvailable == true).FirstOrDefaultAsync();
            if (user == null) return new NotFoundResult();

            this.TargetUser = new UserModel();
            this.TargetUser.UserId = user.UserId;
            this.TargetUser.DisplayName = user.DisplayName;
            this.TargetUser.UpdatedAt = user.UpdatedAt;

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            this.PageInitializeAsync();

            if (TargetUser.UserId != this.UserID) return new ForbidResult();

            using (var tran = await this.DBContext.Database.BeginTransactionAsync())
            {
                //特殊ユーザーチェック
                if (AppConstants.UndeliteableUsers.Contains(this.TargetUser.UserId))
                {
                    return new ForbidResult();
                }

                //存在チェック
                var user = await this.DBContext.Users.Where(x => x.UserId == this.TargetUser.UserId).FirstOrDefaultAsync();
                if (user == null)
                {
                    this.MsgCategory = MsgCategoryDomain.Error;
                    this.Msg = メッセージ.選択対象なし;
                    return this.Page();
                }

                // 既更新チェック
                if (user.UpdatedAt.TruncMillSecond() != TargetUser.UpdatedAt)
                {
                    this.MsgCategory = MsgCategoryDomain.Error;
                    this.Msg = メッセージ.既更新;
                    return this.Page();
                }

                var val = Validation();
                if (val != null)
                {
                    this.MsgCategory = MsgCategoryDomain.Error;
                    this.Msg = val;
                    return this.Page();
                }
            
                user.Password = SHA256Util.GetHashedString(this.TargetUser.Password);
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = this.UserID;

                try
                {
                    await this.DBContext.SaveChangesAsync();
                    tran.Commit();
                    this.MsgCategory = MsgCategoryDomain.Success;
                    this.Msg = メッセージ.更新成功;
                    return this.Page();
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
            //パスワードは空以外
            if (this.TargetUser.Password == null)
            {
                return "パスワードを入力してください。";
            }

            //パスワードが確認用と異なる
            if (this.TargetUser.Password != this.TargetUser.RePassword)
            {
                return "パスワードが確認用と異なります。";
            }

            //パスワードポリシー
            if (!Regex.IsMatch(this.TargetUser.Password, @"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])[0-9a-zA-Z\-]{6,}$"))
            {
                return "パスワードは６文字以上で数字・半角大文字・半角小文字の混在でなければなりません。";
            }

            return null;
        }
    }
}