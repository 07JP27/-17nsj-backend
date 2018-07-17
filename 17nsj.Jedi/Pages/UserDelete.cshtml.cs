using System;
using System.Linq;
using System.Threading.Tasks;
using _17nsj.Jedi.Constants;
using _17nsj.Jedi.Domains;
using _17nsj.Jedi.Models;
using _17nsj.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace _17nsj.Jedi.Pages
{
    [Authorize(Roles=UserRoleDomain.Admin)]
    public class UserDeleteModel : PageModelBase
    {
        public UserDeleteModel(JediDbContext dbContext)
            : base(dbContext)
        {

        }

        [BindProperty]
        public UserModel TargetUser { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null) return new NotFoundResult();

            this.PageInitializeAsync();

            var user = await this.DBContext.Users.Where(x => x.IsAvailable && x.UserId == id).FirstOrDefaultAsync();

            if (user == null) return new NotFoundResult();

            //特殊ユーザーチェック
            if(AppConstants.UndeliteableUsers.Contains(id))
            {
                return new ForbidResult();
            }

            this.TargetUser = new UserModel();
            this.TargetUser.UserId = user.UserId;
            this.TargetUser.DisplayName = user.DisplayName;
            this.TargetUser.UserId = user.UserId;
            this.TargetUser.UserId = user.UserId;

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //特殊ユーザーチェック
            if (AppConstants.UndeliteableUsers.Contains(TargetUser.UserId))
            {
                return new ForbidResult();
            }

            using (var tran = await this.DBContext.Database.BeginTransactionAsync())
            {
                if (TargetUser.UserId == null) return new NotFoundResult();

                this.PageInitializeAsync();

                var user = await this.DBContext.Users.Where(x => x.IsAvailable && x.UserId == TargetUser.UserId).FirstOrDefaultAsync();
                if (user == null)
                {
                    //対象なしエラー
                    this.MsgCategory = MsgCategoryDomain.Error;
                    this.Msg = メッセージ.選択対象なし;
                    return this.Page();
                }

                try
                {
                    this.DBContext.Users.Remove(user);
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
}