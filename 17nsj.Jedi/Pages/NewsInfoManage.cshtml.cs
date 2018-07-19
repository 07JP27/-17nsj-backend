﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _17nsj.Jedi.Domains;
using _17nsj.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _17nsj.Jedi.Pages
{
    [Authorize(UserRoleDomain.SysAdmin)]
    public class NewsInfoManageModel : PageModelBase
    {
        public NewsInfoManageModel(JediDbContext dbContext)
            : base(dbContext)
        {

        }

        public void OnGet()
        {

        }
    }
}