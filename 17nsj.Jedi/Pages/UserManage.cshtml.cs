using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _17nsj.Jedi.Models;
using _17nsj.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace _17nsj.Jedi.Pages
{
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
                this.IsEditMode = true;

                var user = await this.DBContext.Users.Where(x => x.UserId == id && x.IsAvailable == true).FirstOrDefaultAsync();
                if (user == null) return new NotFoundResult();

                TargetUser.UserId = user.UserId;
                TargetUser.DisplayName = user.DisplayName;
                TargetUser.Password = user.Password;
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
                this.IsEditMode = false;
            }

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            this.PageInitializeAsync();
            


            return this.Page();
        }
    }
}