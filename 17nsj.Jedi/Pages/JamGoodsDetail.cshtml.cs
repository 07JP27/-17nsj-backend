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
    public class JamGoodsDetailModel : PageModelBase
    {
        public JamGoodsDetailModel(JediDbContext dbContext)
            : base(dbContext)
        {

        }

        public JamGoodsModel CurrentGoods { get; private set; }

        public async Task<IActionResult> OnGetAsync(string category, int? id)
        {
            if (category == null || id == null) return new RedirectResult("/NotFound");

            this.PageInitializeAsync();

            var goods = await this.DBContext.JamGoods.Where(x => x.IsAvailable && x.Category == category && x.Id == (int)id).FirstOrDefaultAsync();

            if (goods == null) return new RedirectResult("/NotFound");

            this.CurrentGoods = new JamGoodsModel();
            this.CurrentGoods.Category = goods.Category;
            this.CurrentGoods.CategoryName = await this.DBContext.JamGoodsCategories.Where(x => x.Category == goods.Category).Select(x => x.CategoryName).FirstOrDefaultAsync();   
            this.CurrentGoods.Id = goods.Id;
            this.CurrentGoods.DisplayOrder = goods.DisplayOrder;
            this.CurrentGoods.ThumbnailURL = goods.ThumbnailURL;
            this.CurrentGoods.DetailImageURL = goods.DetailImageURL;
            this.CurrentGoods.Stock = goods.Stock;
            this.CurrentGoods.StockUpdatedAt = goods.StockUpdatedAt;
            this.CurrentGoods.PartsNumber = goods.PartsNumber;
            this.CurrentGoods.GoodsName = goods.GoodsName;
            this.CurrentGoods.Price = goods.Price;
            this.CurrentGoods.Size = goods.Size;
            this.CurrentGoods.Description = goods.Description;

            return this.Page();
        }
    }
}