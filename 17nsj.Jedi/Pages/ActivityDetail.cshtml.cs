using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _17nsj.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _17nsj.Jedi.Pages
{
    public class ActivityDetailModel : PageModelBase
    {
        public ActivityDetailModel(JediDbContext dbContext)
            : base(dbContext)
        {

        }

        public async Task<IActionResult> OnGetAsync(string category, int? id)
        {
            if (category == null || id == null) return new NotFoundResult();

            this.PageInitializeAsync();

            return this.Page();
        }
    }
}