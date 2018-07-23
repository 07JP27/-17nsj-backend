using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _17nsj.Jedi.Constants;
using _17nsj.Jedi.Domains;
using _17nsj.Jedi.Extensions;
using _17nsj.Jedi.Models;
using _17nsj.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace _17nsj.Jedi.Pages
{
    [Authorize(Roles = UserRoleDomain.SysAdmin)]
    public class MovieDeleteModel : PageModelBase
    {
        public MovieDeleteModel(JediDbContext dbContext)
            : base(dbContext)
        {

        }

        [BindProperty]
        public MovieModel CurrentMovie { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return new NotFoundResult();

            this.PageInitializeAsync();

            var news = await this.DBContext.Movies.Where(x => x.Id == (int)id && x.IsAvailable == true).FirstOrDefaultAsync();

            if (news == null) return new NotFoundResult();

            this.CurrentMovie = new MovieModel();
            CurrentMovie.Id = news.Id;
            CurrentMovie.Title = news.Title;
            CurrentMovie.UpdatedAt = news.UpdatedAt;

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            using (var tran = await this.DBContext.Database.BeginTransactionAsync())
            {
                if (CurrentMovie.Id == 0) return new NotFoundResult();

                this.PageInitializeAsync();

                var movie = await this.DBContext.Movies.Where(x => x.Id == CurrentMovie.Id && x.IsAvailable == true).FirstOrDefaultAsync();
                if (movie == null)
                {
                    //対象なしエラー
                    this.MsgCategory = MsgCategoryDomain.Error;
                    this.Msg = メッセージ.選択対象なし;
                    return this.Page();
                }

                // 既更新チェック
                if (movie.UpdatedAt.TruncMillSecond() != CurrentMovie.UpdatedAt)
                {
                    this.MsgCategory = MsgCategoryDomain.Error;
                    this.Msg = メッセージ.既更新;
                    return this.Page();
                }

                movie.IsAvailable = false;
                movie.UpdatedAt = DateTime.UtcNow;
                movie.UpdatedBy = this.UserID;

                try
                {
                    await this.DBContext.SaveChangesAsync();
                    tran.Commit();
                    return new RedirectResult("/MovieList");
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    this.MsgCategory = MsgCategoryDomain.Error;
                    this.Msg = ex.Message;
                    return this.Page();
                }
            }
        }
    }
}