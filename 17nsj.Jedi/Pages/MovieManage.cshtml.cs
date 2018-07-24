using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _17nsj.DataAccess;
using _17nsj.Jedi.Constants;
using _17nsj.Jedi.Domains;
using _17nsj.Jedi.Extensions;
using _17nsj.Jedi.Models;
using _17nsj.Jedi.Utils;
using _17nsj.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace _17nsj.Jedi.Pages
{
    [Authorize(Roles = UserRoleDomain.SysAdmin)]
    public class MovieManageModel : PageModelBase
    {
        private ILogger _logger;

        public MovieManageModel(JediDbContext dbContext, ILogger<MovieManageModel> logger)
            : base(dbContext)
        {
            _logger = logger;
        }

        [BindProperty]
        public MovieModel TargetMovie { get; set; }

        [BindProperty]
        public bool IsEditMode { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            this.PageInitializeAsync();

            if (id != null)
            {
                // 既存更新
                this.IsEditMode = true;

                var movie = await this.DBContext.Movies.Where(x => x.Id == id && x.IsAvailable == true).FirstOrDefaultAsync();
                if (movie == null) return new RedirectResult("/NotFound");

                TargetMovie = new MovieModel();
                TargetMovie.Id = movie.Id;
                TargetMovie.Title = movie.Title;
                TargetMovie.Outline = movie.Outline;
                TargetMovie.ThumbnailURL = movie.ThumbnailURL;
                TargetMovie.URL = movie.URL;
                TargetMovie.UpdatedAt = movie.UpdatedAt;
            }
            else
            {
                // 新規作成
                this.IsEditMode = false;
            }

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            this.PageInitializeAsync();

            if (this.IsEditMode)
            {
                var val = Validation();
                if (val != null)
                {
                    this.MsgCategory = MsgCategoryDomain.Error;
                    this.Msg = val;
                    return this.Page();
                }

                //更新
                using (var tran = await this.DBContext.Database.BeginTransactionAsync())
                {
                    //存在チェック
                    var movie = await this.DBContext.Movies.Where(x => x.Id == this.TargetMovie.Id && x.IsAvailable == true).FirstOrDefaultAsync();
                    if (movie == null)
                    {
                        this.MsgCategory = MsgCategoryDomain.Error;
                        this.Msg = メッセージ.選択対象なし;
                        return this.Page();
                    }

                    // 既更新チェック
                    if (movie.UpdatedAt.TruncMillSecond() != this.TargetMovie.UpdatedAt)
                    {
                        this.MsgCategory = MsgCategoryDomain.Error;
                        this.Msg = メッセージ.既更新;
                        return this.Page();
                    }

                    movie.Title = this.TargetMovie.Title;
                    movie.Outline = this.TargetMovie.Outline;
                    movie.URL = this.TargetMovie.URL;
                    movie.ThumbnailURL = this.TargetMovie.ThumbnailURL;
                    movie.UpdatedAt = DateTime.UtcNow;
                    movie.UpdatedBy = this.UserID;

                    try
                    {
                        await this.DBContext.SaveChangesAsync();
                        tran.Commit();
                        _logger.LogInformation($"【ムービー更新】ユーザー：{this.UserID}　対象：{this.TargetMovie.Id}");
                        return new RedirectResult($"/MovieList");
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        _logger.LogError(ex, $"【ムービー更新エラー】ユーザー：{this.UserID}　対象：{this.TargetMovie.Id}");
                        this.MsgCategory = MsgCategoryDomain.Error;
                        this.Msg = ex.Message;
                        return this.Page();
                    }
                }
            }
            else
            {
                var val = Validation();
                if (val != null)
                {
                    this.MsgCategory = MsgCategoryDomain.Error;
                    this.Msg = val;
                    return this.Page();
                }

                // 新規作成
                using (var tran = await this.DBContext.Database.BeginTransactionAsync())
                {
                    //最大ID取得
                    var maxId = await this.DBContext.Movies.MaxAsync(x => x.Id);
                    this.TargetMovie.Id = maxId + 1;

                    var entity = new Movies();
                    var now = DateTime.UtcNow;
                    entity.Id = this.TargetMovie.Id;
                    entity.Title = this.TargetMovie.Title;
                    entity.Outline = this.TargetMovie.Outline;
                    entity.URL = this.TargetMovie.URL;
                    entity.ThumbnailURL = this.TargetMovie.ThumbnailURL;
                    entity.IsAvailable = true;
                    entity.CreatedAt = now;
                    entity.CreatedBy = this.UserID;
                    entity.UpdatedAt = now;
                    entity.UpdatedBy = this.UserID;

                    try
                    {
                        await this.DBContext.Movies.AddAsync(entity);
                        await this.DBContext.SaveChangesAsync();
                        tran.Commit();
                        _logger.LogInformation($"【ムービー登録】ユーザー：{this.UserID}　対象：{this.TargetMovie.Id}");
                        return new RedirectResult($"/MovieList");
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        _logger.LogError(ex, $"【ムービー登録エラー】ユーザー：{this.UserID}　対象：{this.TargetMovie.Id}");
                        this.MsgCategory = MsgCategoryDomain.Error;
                        this.Msg = ex.Message;
                        return this.Page();
                    }
                }
            }
        }

        private string Validation()
        {
            //タイトルは1~30文字以内
            if (this.TargetMovie.Title == null || this.TargetMovie.Title.Length <= 0 || this.TargetMovie.Title.Length >= 30)
            {
                return "タイトルは1~30文字で入力してください。";
            }

            if (this.TargetMovie.ThumbnailURL == null || !URLUtil.IsUrl(this.TargetMovie.ThumbnailURL) || this.TargetMovie.ThumbnailURL.Length <= 0 || this.TargetMovie.ThumbnailURL.Length >= 100)
            {
                return "サムネイルURLは1～200文字の正しいURLの形式で入力してください。";
            }

            if (this.TargetMovie.URL == null || !URLUtil.IsUrl(this.TargetMovie.URL) || this.TargetMovie.URL.Length <= 0 || this.TargetMovie.URL.Length >= 100)
            {
                return "ムービーURLは1～200文字の正しいURLの形式で入力してください。";
            }

            if (this.TargetMovie.Outline == null || this.TargetMovie.Outline.Length <= 0 || this.TargetMovie.Outline.Length >= 100)
            {
                return "説明は1~100文字で入力してください。";
            }

            return null;
        }
    }
}