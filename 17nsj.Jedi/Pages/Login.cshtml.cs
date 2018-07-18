using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _17nsj.Jedi.Models;
using _17nsj.Repository;
using Microsoft.EntityFrameworkCore;
using _17nsj.Jedi.Utils;
using _17nsj.Jedi.Domains;
using _17nsj.DataAccess;

namespace _17nsj.Jedi.Pages
{
    public class LoginModel : PageModel
    {
        public LoginModel(JediDbContext dbContext)
        {
            this.DBContext = dbContext;
        }

        protected JediDbContext DBContext { get; private set; }

        [BindProperty]
        public LoginDataModel loginData { get; set; }

        public string Msg { get; set; }
        public int MsgCategory { get; set; }

        public async Task<IActionResult> OnPostAsync(string ReturnUrl)
        {
            var user = await this.DBContext.Users.Where(x => x.UserId == loginData.Username && x.IsAvailable == true).FirstOrDefaultAsync();

            if (user == null)
            {
                this.Msg = "ユーザーIDまたはパスワードが無効です。";
                this.MsgCategory = MsgCategoryDomain.Error;
                return Page();
            }

            var isValid = (user.Password == SHA256Util.GetHashedString(loginData.Password).ToLower());
            if (!isValid)
            {
                this.Msg = "ユーザーIDまたはパスワードが無効です。";
                this.MsgCategory = MsgCategoryDomain.Error;
                return Page();
            }

            var identity = new System.Security.Claims.ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.UserId));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.DisplayName));
            identity.AddClaim(new Claim(ClaimTypes.Role, GetUserRole(user)));
            identity.AddClaim(new Claim(ClaimTypes.GroupSid, user.Affiliation == null ? string.Empty : user.Affiliation));
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { });

            if (ReturnUrl == null || ReturnUrl == "/")
            {
                return RedirectToPage("Index");
            }
            else
            {
                return RedirectToPage(ReturnUrl);
            }

        }

        private string GetUserRole(Users user)
        {
            if (user.IsSysAdmin)
            {
                return UserRoleDomain.SysAdmin;
            }

            if (user.IsAdmin)
            {
                return UserRoleDomain.Admin;
            }

            if(user.CanWrite)
            {
                return UserRoleDomain.Writer;
            }

            return UserRoleDomain.Reader;
        }
    }
}