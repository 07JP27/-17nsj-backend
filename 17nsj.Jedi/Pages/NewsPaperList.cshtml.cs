using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _17nsj.Jedi.Models;
using _17nsj.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace _17nsj.Jedi.Pages
{
    public class NewsPaperListModel : PageModelBase
    {
        public NewsPaperListModel(JediDbContext dbContext)
            : base(dbContext)
        {

        }

        public List<NewsPaperModel> NPリスト { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            this.PageInitializeAsync();

            var newspapers = await this.DBContext.Newspapers.ToListAsync();

            NPリスト = new List<NewsPaperModel>();
            foreach (var item in newspapers)
            {
                var model = new NewsPaperModel();
                model.Id = item.Id;
                model.Title = item.Title;
                model.URL = item.URL;
                model.ThumbnailURL = item.ThumbnailURL;
                model.CreatedAt = item.CreatedAt;
                this.NPリスト.Add(model);
            }

            return this.Page();
        }
    }
}