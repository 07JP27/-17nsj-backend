using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _17nsj.DataAccess;
using _17nsj.Jedi.Domains;
using _17nsj.Jedi.Models;
using _17nsj.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace _17nsj.Jedi.Pages
{
    [Authorize(Roles=UserRoleDomain.Admin + "," + UserRoleDomain.SysAdmin)]
    public class NoticeBoardListModel : PageModelBase
    {
        public NoticeBoardListModel(JediDbContext dbContext)
            : base(dbContext)
        {

        }

        public List<NoticeModel> お知らせリスト { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            this.PageInitializeAsync();
            IQueryable<NoticeBoard> query;

            if(IsSysAdmin)
            {
                //システム管理者はすべて
                query = this.DBContext.NoticeBoard.OrderByDescending(x => x.UpdatedAt);
            }
            else
            {
                //管理者は自分の作成したものだけ
                query = this.DBContext.NoticeBoard.Where(x => x.CreatedBy == this.UserID).OrderByDescending(x => x.UpdatedAt);
            }

            お知らせリスト = new List<NoticeModel>();
            foreach (var item in await query.ToListAsync())
            {
                var model = new NoticeModel();
                model.Id = item.Id;
                model.Title = item.Title;
                model.Contents = item.Contents;
                model.Receiver = item.Receiver;
                model.Sender = item.Sender;
                model.Termination = item.Termination;
                model.CreatedAt = item.CreatedAt;
                this.お知らせリスト.Add(model);
            }

            return this.Page();
        }
    }
}