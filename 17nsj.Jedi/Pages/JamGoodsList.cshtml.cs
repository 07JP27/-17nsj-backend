using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _17nsj.Jedi.Domains;
using _17nsj.Jedi.Models;
using _17nsj.Repository;
using Microsoft.AspNetCore.Authorization;
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

        public List<JamGoodsByCategoryModel> グッズリスト { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            this.PageInitializeAsync();

            var categories = await this.DBContext.JamGoodsCategories.ToListAsync();
            var goods = await this.DBContext.JamGoods.Where(x => x.IsAvailable == true).Select(x => new { x.Category, x.Id, x.ThumbnailURL, x.GoodsName, x.DisplayOrder, x.Price, x.Stock }).ToListAsync();

            グッズリスト = new List<JamGoodsByCategoryModel>();
            foreach(var cat in categories.OrderBy(x => x.DisplayOrder))
            {
                var model = new JamGoodsByCategoryModel();
                model.CategoryName = cat.CategoryName;
                model.Goods = new List<JamGoodsModel>();

                foreach(var item in goods.Where(x => x.Category == cat.Category).OrderBy(x => x.DisplayOrder))
                {
                    var itemModel = new JamGoodsModel();
                    itemModel.Category = item.Category;
                    itemModel.Id = item.Id;
                    itemModel.GoodsName = item.GoodsName;
                    itemModel.ThumbnailURL = item.ThumbnailURL;
                    itemModel.Price = item.Price;
                    itemModel.Stock = item.Stock;
                    model.Goods.Add(itemModel);
                }

                グッズリスト.Add(model);
            }

            return this.Page();
        }
    }
}