using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _17nsj.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _17nsj.Jedi.Pages.Account
{
    public class AccessDeniedModel : PageModelBase
    {
        public AccessDeniedModel(JediDbContext dbContext)
            : base(dbContext)
        {

        }

        public IActionResult OnGet()
        {
            this.PageInitializeAsync();
            return this.Page();
        }
    }
}