using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _17nsj.Jedi.Domains;
using _17nsj.Jedi.Models;
using _17nsj.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace _17nsj.Jedi.Pages
{
    [Authorize(Roles = UserRoleDomain.Writer + "," + UserRoleDomain.Admin + "," + UserRoleDomain.SysAdmin)]
    public class NewsInfoCompleteRegisterModel : PageModelBase
    {
        public NewsInfoCompleteRegisterModel(JediDbContext dbContext)
           : base(dbContext)
        {

        }

        public NewsModel CurrentNews { get; private set; }

        public async Task<IActionResult> OnGetAsync(string category, int? id)
        {
            if (category == null || id == null) return new RedirectResult("/NotFound");
            this.PageInitializeAsync();

            var news = await this.DBContext.News.Where(x => x.IsAvailable == true && x.Category == category && x.Id == (int)id).FirstOrDefaultAsync();

            if (news == null) return new RedirectResult("/NotFound");

            this.CurrentNews = new NewsModel();
            CurrentNews.Category = news.Category;
            CurrentNews.Id = news.Id;
            return this.Page();
        }
    }
}