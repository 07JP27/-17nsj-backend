using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _17nsj.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _17nsj.Jedi.Pages
{
    public class JamGoodsListModel : PageModelBase
    {
        public JamGoodsListModel(JediDbContext dbContext)
            : base(dbContext)
        {

        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!this.IsAdmin) return new ForbidResult();

            this.PageInitializeAsync();
            return this.Page();
        }
    }
}