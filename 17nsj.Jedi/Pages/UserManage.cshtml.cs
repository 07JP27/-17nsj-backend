using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using Microsoft.EntityFrameworkCore;

namespace _17nsj.Jedi.Pages
{
    [Authorize(Roles=UserRoleDomain.Admin + "," + UserRoleDomain.SysAdmin)]
    public class UserManageModel : PageModelBase
    {
        public UserManageModel(JediDbContext dbContext)
            : base(dbContext)
        {

        }

        [BindProperty]
        public UserModel TargetUser { get; set; }

        [BindProperty]
        public bool IsEditMode { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            this.PageInitializeAsync();

            if (!string.IsNullOrEmpty(id))
            {
                // 既存更新
                this.IsEditMode = true;

                var user = await this.DBContext.Users.Where(x => x.UserId == id && x.IsAvailable == true).FirstOrDefaultAsync();
                if (user == null)
                {
                    //対象なしエラー
                    this.MsgCategory = MsgCategoryDomain.Error;
                    this.Msg = メッセージ.選択対象なし;
                    return this.Page();
                }

                //特殊ユーザーチェック
                if (AppConstants.UndeliteableUsers.Contains(id))
                {
                    return new ForbidResult();
                }

                TargetUser = new UserModel();
                var now = DateTime.UtcNow;
                TargetUser.UserId = user.UserId;
                TargetUser.DisplayName = user.DisplayName;
                TargetUser.Affiliation = user.Affiliation;
                TargetUser.CanRead = user.CanRead;
                TargetUser.CanWrite = user.CanWrite;
                TargetUser.IsAdmin = user.IsAdmin;
                TargetUser.IsSysAdmin = user.IsSysAdmin;
                TargetUser.IsAvailable = user.IsAvailable;
                TargetUser.CreatedAt = user.CreatedAt;
                TargetUser.CreatedBy = user.CreatedBy;
                TargetUser.UpdatedAt = user.UpdatedAt;
                TargetUser.UpdatedBy = user.UpdatedBy;
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
                    if(user.UpdatedAt.TruncMillSecond() != TargetUser.UpdatedAt)
                    {
                        this.MsgCategory = MsgCategoryDomain.Error;
                        this.Msg = メッセージ.既更新;
                        return this.Page();
                    }

                    if (!this.IsSysAdmin)
                    {
                        user.Affiliation = this.TargetUser.Affiliation;
                    }
                    else
                    {
                        //システム管理者でなければ自分の所属と同じユーザー
                        user.Affiliation = this.UserAffiliation;
                    }
                    user.DisplayName = this.TargetUser.DisplayName;
                    user.CanRead = this.TargetUser.CanRead;
                    user.CanWrite = this.TargetUser.CanWrite;
                    user.UpdatedAt = DateTime.UtcNow;
                    user.UpdatedBy = this.UserID;

                    try
                    {
                        await this.DBContext.SaveChangesAsync();
                        tran.Commit();
                        return new RedirectResult("/UserList");
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
                    //存在チェック
                    var exist = await this.DBContext.Users.Where(x => x.UserId == this.TargetUser.UserId).AnyAsync();
                    if(exist)
                    {
                        this.MsgCategory = MsgCategoryDomain.Error;
                        this.Msg = メッセージ.ユーザーID重複;
                        return this.Page();
                    }

                    var entity = new Users();
                    var now = DateTime.UtcNow;
                    entity.UserId = this.TargetUser.UserId;
                    entity.DisplayName = this.TargetUser.DisplayName;

                    if(this.IsSysAdmin)
                    {
                        entity.Affiliation = this.TargetUser.Affiliation;
                    }
                    else
                    {
                        //システム管理者でなければ自分の所属と同じユーザー
                        entity.Affiliation = this.UserAffiliation;
                    }
                    entity.Password = SHA256Util.GetHashedString(this.TargetUser.Password);
                    entity.CanRead = this.TargetUser.CanRead;
                    entity.CanWrite = this.TargetUser.CanWrite;
                    entity.IsAdmin = this.TargetUser.IsAdmin;
                    entity.IsAvailable = true;
                    entity.CreatedAt = now;
                    entity.CreatedBy = this.UserID;
                    entity.UpdatedAt = now;
                    entity.UpdatedBy = this.UserID;

                    try
                    {
                        await this.DBContext.Users.AddAsync(entity);
                        await this.DBContext.SaveChangesAsync();
                        tran.Commit();
                        return new RedirectResult("/UserList");
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
            //ユーザーIDは半角1~30文字
            if (this.TargetUser.UserId == null || this.TargetUser.UserId.Length <= 0 || this.TargetUser.UserId.Length >= 30　|| Regex.IsMatch(this.TargetUser.UserId, @"[^a-zA-z0-9]"))
            {
                return "ユーザーIDは半角1~30文字で入力してください。";
            }

            //表示名は1~30文字以内
            if (this.TargetUser.DisplayName == null || this.TargetUser.DisplayName.Length <= 0 || this.TargetUser.DisplayName.Length >= 30)
            {
                return "表示名は1~30文字で入力してください。";
            }

            //パスワードは空以外
            if (this.TargetUser.Password == null)
            {
                return "初期パスワードを入力してください。";
            }

            //パスワードが確認用と異なる
            if (this.TargetUser.Password != this.TargetUser.RePassword)
            {
                return "初期パスワードが確認用と異なります。";
            }

            //パスワードポリシー
            if (!Regex.IsMatch(this.TargetUser.Password, @"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])[0-9a-zA-Z\-]{6,}$"))
            {
                return "パスワードは６文字以上で数字・半角大文字・半角小文字の混在でなければなりません。";
            }

            return null;
        }

        private string UpdateValidation()
        {
            //表示名は1~30文字以内
            if (this.TargetUser.DisplayName == null || this.TargetUser.DisplayName.Length <= 0 || this.TargetUser.DisplayName.Length >= 30)
            {
                return "表示名は1~30文字で入力してください。";
            }

            return null;
        }
    }
}