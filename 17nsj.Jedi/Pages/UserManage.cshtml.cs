using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
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

        private bool IsEditMode { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            this.PageInitializeAsync();

            if (string.IsNullOrEmpty(id))
            {
                // 既存更新
                this.IsEditMode = true;

                var user = await this.DBContext.Users.Where(x => x.UserId == id && x.IsAvailable == true).FirstOrDefaultAsync();
                if (user == null)
                {
                    //対象なしエラー
                    return this.Page();
                }

                if(user.UpdatedAt != TargetUser.UpdatedAt)
                {
                    //既更新エラー
                    return this.Page();
                }

                var now = DateTime.UtcNow;
                TargetUser.UserId = user.UserId;
                TargetUser.DisplayName = user.DisplayName;
                TargetUser.Password = SHA256Util.GetHashedString(user.Password);
                TargetUser.CanRead = user.CanRead;
                TargetUser.CanWrite = user.CanWrite;
                TargetUser.IsAdmin = user.IsAdmin;
                TargetUser.IsAvailable = true;
                TargetUser.CreatedAt = now;
                TargetUser.CreatedBy = this.UserID;
                TargetUser.UpdatedAt = now;
                TargetUser.UpdatedBy = this.UserID;
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

            }
            else
            {
                using (var tran = await this.DBContext.Database.BeginTransactionAsync())
                {
                    //存在チェック
                    var exist = await this.DBContext.Users.Where(x => x.UserId == this.TargetUser.UserId).AnyAsync();
                    if(exist)
                    {
                        this.MsgCategory = MsgCategoryDomain.Error;
                        this.Msg = "";
                        return this.Page();
                    }

                

                    tran.Commit();
                    tran.Rollback();
                }
            }
            


            return this.Page();
        }
    }
}