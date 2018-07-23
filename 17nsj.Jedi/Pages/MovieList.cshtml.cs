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
    public class MovieListModel : PageModelBase
    {
        public MovieListModel(JediDbContext dbContext)
            : base(dbContext)
        {

        }

        public List<MovieModel> ムービーリスト { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            this.PageInitializeAsync();
            var movies = await this.DBContext.Movies.Where(x => x.IsAvailable)
                .Select(x => new {x.Id, x.Title, x.Outline, x.ThumbnailURL, x.URL }).OrderByDescending(x => x.Id).ToListAsync();

            ムービーリスト = new List<MovieModel>();
            foreach (var item in movies)
            {
                var model = new MovieModel();
                model.Id = item.Id;
                model.Title = item.Title;
                model.Outline = item.Outline;
                model.ThumbnailURL = item.ThumbnailURL;
                model.URL = item.URL;
                ムービーリスト.Add(model);
            }

            return this.Page();
        }
    }
}