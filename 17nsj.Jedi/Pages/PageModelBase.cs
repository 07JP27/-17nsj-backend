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

        public string UserID
        {
            get
            {
                return this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
        }

        public string UserName
        {
            get
            {
                return this.User.FindFirst(ClaimTypes.Name).Value;
            }
        }

        public string UserRole
        {
            get
            {
                return UserRoleDomain.GetName(this.User.FindFirst(ClaimTypes.Role).Value);
            }
        }

        public bool CanRead { get; private set; }
        public bool CanWrite { get; private set; }
        public bool IsAdmin { get; private set; }
        public string Msg { get; set; }
        public int MsgCategory { get; set; }

        protected void PageInitializeAsync()
        {
            var role = this.User.FindFirst(ClaimTypes.Role).Value;
            if (role == UserRoleDomain.Admin)
            {
                this.IsAdmin = true;
                this.CanWrite = true;
                this.CanRead = true;
            }
            else if(this.UserRole == UserRoleDomain.Writer)
            {
                this.IsAdmin = false;
                this.CanWrite = true;
                this.CanRead = true;
            }
            else
            {
                this.IsAdmin = false;
                this.CanWrite = false;
                this.CanRead = true;
            }
        }

        public async Task<IActionResult> OnPostSignOutAsync()
        {
            await HttpContext.SignOutAsync();
            return RedirectToPage("/Login");
        }
    }
}
