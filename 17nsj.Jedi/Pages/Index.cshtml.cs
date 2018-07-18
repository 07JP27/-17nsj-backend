using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _17nsj.Jedi.Domains;
using _17nsj.Jedi.Models;
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

        public List<NoticeModel> お知らせリスト { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            this.PageInitializeAsync();

            var notices = await this.DBContext.NoticeBoard.Where(x => x.Termination >= DateTime.UtcNow).OrderByDescending(x => x.UpdatedAt).ToListAsync();

            お知らせリスト = new List<NoticeModel>();
            foreach (var item in notices)
            {
                var model = new NoticeModel();
                model.Id = item.Id;
                model.Title = item.Title;
                model.Contents = item.Contents;
                model.Receiver = item.Receiver;
                model.Sender = item.Sender;
                model.CreatedAt = item.CreatedAt;
                this.お知らせリスト.Add(model);
            }

            return this.Page();
        }
    }
}
