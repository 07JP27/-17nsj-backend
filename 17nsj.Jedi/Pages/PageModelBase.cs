using _17nsj.Repository;
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

        protected async Task PageInitializeAsync()
        {
            var a = this.User.FindFirst(ClaimTypes.Name).Value;
            var b = this.User.FindFirst(ClaimTypes.Role).Value;
            var notifitcations = await this.DBContext.News.ToListAsync();
        }
    }
}
