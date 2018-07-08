using _17nsj.Jedi.Domains;
using _17nsj.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace _17nsj.Jedi.Pages
{
    public class PageModelBase : PageModel
    {
        public PageModelBase(JediDbContext dbContext)
        {
            this.DBContext = dbContext;
        }

        protected JediDbContext DBContext { get; private set; }

        protected string UserID
        {
            get
            {
                return this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
        }

        protected string UserName
        {
            get
            {
                return this.User.FindFirst(ClaimTypes.Name).Value;
            }
        }

        protected string UserRole
        {
            get
            {
                return UserRoleDomain.GetName(this.User.FindFirst(ClaimTypes.Role).Value);
            }
        }

        protected bool CanRead { get; private set; }
        protected bool CanWrite { get; private set; }
        protected bool IsAdmin { get; private set; }
        protected string Msg { get; set; }
        protected int MsgCategory { get; set; }

        protected void PageInitializeAsync()
        {
            if(this.UserRole == UserRoleDomain.Admin)
            {
                this.IsAdmin = true;
                this.CanRead = true;
                this.CanWrite = true;
            }
            else if(this.UserRole == UserRoleDomain.Writer)
            {
                this.IsAdmin = false;
                this.CanRead = true;
                this.CanWrite = true;
            }
            else
            {
                this.IsAdmin = false;
                this.CanRead = false;
                this.CanWrite = true;
            }
        }

        public async Task<IActionResult> OnPostSignOutAsync()
        {
            await HttpContext.SignOutAsync();
            return RedirectToPage("Login");
        }
    }
}
