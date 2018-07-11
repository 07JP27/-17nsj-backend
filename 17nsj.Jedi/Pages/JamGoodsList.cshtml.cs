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
    public class JamGoodsListModel : PageModelBase
    {
        public JamGoodsListModel(JediDbContext dbContext)
            : base(dbContext)
        {

        }

        public List<JamGoodsModel> グッズリスト { get; set; }
        public List<JamGoodsCategoriesModel> カテゴリーリスト { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!this.IsAdmin) return new ForbidResult();
            this.PageInitializeAsync();

            var categories = await this.DBContext.JamGoodsCategories.ToListAsync();
            var goods = await this.DBContext.JamGoods.Where(x => x.IsAvailable == true).Select(x => new { x.Category, x.Id, x.ThumbnailURL, x.GoodsName, x.Description }).ToListAsync();

            カテゴリーリスト = new List<JamGoodsCategoriesModel>();
            foreach (var item in categories)
            {
                var model = new JamGoodsCategoriesModel();
                model.Category = item.Category;
                model.CategoryName = item.CategoryName;
                カテゴリーリスト.Add(model);
            }

            グッズリスト = new List<JamGoodsModel>();
            foreach (var item in goods)
            {
                var model = new JamGoodsModel();
                model.Category = item.Category;
                model.Id = item.Id;
                model.ThumbnailURL = item.ThumbnailURL;
                model.GoodsName = item.GoodsName;
                model.Description = item.Description;
                グッズリスト.Add(model);
            }

            return this.Page();
        }
    }
}