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

namespace _17nsj.Jedi.Pages
{
    public class DocumentListModel : PageModelBase
    {
        public DocumentListModel(JediDbContext dbContext)
            : base(dbContext)
        {

        }

        public List<DocumentModel> ドキュメントリスト { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            this.PageInitializeAsync();
            var movies = await this.DBContext.Documents.Where(x => x.IsAvailable)
                .Select(x => new { x.Id, x.Title, x.Outline, x.ThumbnailURL, x.URL }).OrderByDescending(x => x.Id).ToListAsync();

            ドキュメントリスト = new List<DocumentModel>();
            foreach (var item in movies)
            {
                var model = new DocumentModel();
                model.Id = item.Id;
                model.Title = item.Title;
                model.Outline = item.Outline;
                model.ThumbnailURL = item.ThumbnailURL;
                model.URL = item.URL;
                ドキュメントリスト.Add(model);
            }

            return this.Page();
        }
    }
}