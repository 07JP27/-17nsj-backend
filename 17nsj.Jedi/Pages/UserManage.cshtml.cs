using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using _17nsj.DataAccess;
using _17nsj.Jedi.Constants;
using _17nsj.Jedi.Domains;
using _17nsj.Jedi.Models;
using _17nsj.Jedi.Utils;
using _17nsj.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace _17nsj.Jedi.Pages
{
    [Authorize(Roles="2")]
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

                TargetUser = new UserModel();
                var now = DateTime.UtcNow;
                TargetUser.UserId = user.UserId;
                TargetUser.DisplayName = user.DisplayName;
                TargetUser.Password = "・・・・・・・";
                TargetUser.CanRead = user.CanRead;
                TargetUser.CanWrite = user.CanWrite;
                TargetUser.IsAdmin = user.IsAdmin;
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

            if(this.IsEditMode)
            {
                //更新
            }
            else
            {
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
                    entity.UserId = this.TargetUser.UserId;
                    entity.UserId = this.TargetUser.UserId;
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
                        this.MsgCategory = MsgCategoryDomain.Success;
                        this.Msg = メッセージ.作成成功;
                        return this.Page();
                    }
                    catch(Exception ex)
                    {
                        tran.Rollback();
                        this.MsgCategory = MsgCategoryDomain.Error;
                        this.Msg = ex.Message;
                        return this.Page();
                    }
                }
            }
            


            return this.Page();
        }
    }
}