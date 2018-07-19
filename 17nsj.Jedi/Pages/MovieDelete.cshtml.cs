using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _17nsj.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _17nsj.Jedi.Pages
{
    public class MovieDeleteModel : PageModelBase
    {
        public MovieDeleteModel(JediDbContext dbContext)
            : base(dbContext)
        {

        }

        public void OnGet()
        {

        }
    }
}