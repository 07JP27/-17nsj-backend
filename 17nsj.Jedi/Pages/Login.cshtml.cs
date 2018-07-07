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

namespace _17nsj.Jedi.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public LoginDataModel loginData { get; set; }

        public async Task<IActionResult> OnPostAsync(string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var isValid = (loginData.Username == "user" && loginData.Password == "pass");
                if (!isValid)
                {
                    ModelState.AddModelError("", "username or password is invalid");
                    return Page();
                }
                var identity = new System.Security.Claims.ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, loginData.Username));
                identity.AddClaim(new Claim(ClaimTypes.Name, loginData.Username));
                identity.AddClaim(new Claim(ClaimTypes.Role, "Reader"));
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
            else
            {
                ModelState.AddModelError("", "username or password is blank");
                return Page();
            }
        }
    }

    public class LoginDataModel
    {
        [Required]
        public string Username { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }
}