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
    public class NoticeBoardDetailModel : PageModelBase
    {
        public NoticeBoardDetailModel(JediDbContext dbContext)
            : base(dbContext)
        {

        }

        public NoticeModel CurrentNotice { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return new NotFoundResult();
            this.PageInitializeAsync();

            var notice = await this.DBContext.NoticeBoard.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (notice == null) return new NotFoundResult();

            this.CurrentNotice = new NoticeModel();
            this.CurrentNotice.Id = notice.Id;
            this.CurrentNotice.Title = notice.Title;
            this.CurrentNotice.Contents = notice.Contents;
            this.CurrentNotice.Sender = notice.Sender;
            this.CurrentNotice.Receiver = notice.Receiver;
            this.CurrentNotice.CreatedAt = notice.CreatedAt;
            this.CurrentNotice.UpdatedAt = notice.UpdatedAt;

            return this.Page();

        }
    }
}