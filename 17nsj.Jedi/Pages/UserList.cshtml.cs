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
    public class UserListModel : PageModelBase
    {
        public UserListModel(JediDbContext dbContext)
            : base(dbContext)
        {

        }

        public List<UserModel> ユーザーリスト { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            this.PageInitializeAsync();

            IQueryable<Users> query;
            if(IsSysAdmin)
            {
                query = this.DBContext.Users.Where(x => x.IsAvailable == true);
            }
            else
            {
                query = this.DBContext.Users.Where(x => x.IsAvailable && x.Affiliation == this.UserAffiliation);
            }

            ユーザーリスト = new List<UserModel>();
            foreach(var item in await query.ToListAsync())
            {
                var model = new UserModel();
                model.UserId = item.UserId;
                model.DisplayName = item.DisplayName;
                model.Affiliation = item.Affiliation;
                model.CanRead = item.CanRead;
                model.CanWrite = item.CanWrite;
                model.IsAdmin = item.IsAdmin;
                model.IsSysAdmin = item.IsSysAdmin;
                this.ユーザーリスト.Add(model);
            }

            return this.Page();
        }
    }
}