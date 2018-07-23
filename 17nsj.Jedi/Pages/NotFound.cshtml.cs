using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _17nsj.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _17nsj.Jedi.Pages
{
    public class NotFoundModel : PageModelBase
    {
        public NotFoundModel(JediDbContext dbContext)
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