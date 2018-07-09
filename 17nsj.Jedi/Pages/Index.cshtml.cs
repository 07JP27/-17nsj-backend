using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _17nsj.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace _17nsj.Jedi.Pages
{
    public class IndexModel : PageModelBase
    {
        public IndexModel(JediDbContext dbContext)
            :base(dbContext)
        {

        }

        public async Task OnGetAsync()
        {
        }
    }
}
