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
    public class JamGoodsStockManageModel : PageModelBase
    {
        public JamGoodsStockManageModel(JediDbContext dbContext)
            : base(dbContext)
        {

        }

        [BindProperty]
        public JamGoodsModel TargetGoods { get; set; }

        public async Task<IActionResult> OnGetAsync(string category, int? id)
        {
            if (category == null || id == null) return new NotFoundResult();

            this.PageInitializeAsync();

            var goods = await this.DBContext.JamGoods.Where(x => x.IsAvailable && x.Category == category && x.Id == (int)id).FirstOrDefaultAsync();

            if (goods == null) return new NotFoundResult();

            this.TargetGoods = new JamGoodsModel();
            this.TargetGoods.Category = goods.Category;
            this.TargetGoods.CategoryName = await this.DBContext.JamGoodsCategories.Where(x => x.Category == goods.Category).Select(x => x.CategoryName).FirstOrDefaultAsync();
            this.TargetGoods.Id = goods.Id;
            this.TargetGoods.PartsNumber = goods.PartsNumber;
            this.TargetGoods.GoodsName = goods.GoodsName;
            this.TargetGoods.Stock = goods.Stock;
            this.TargetGoods.UpdatedAt = goods.UpdatedAt;
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            this.PageInitializeAsync();

            using (var tran = await this.DBContext.Database.BeginTransactionAsync())
            {
                //存在チェック
                var goods = await this.DBContext.JamGoods.Where(x => x.IsAvailable && x.Category == this.TargetGoods.Category && x.Id == this.TargetGoods.Id).FirstOrDefaultAsync();
                if (goods == null)
                {
                    this.MsgCategory = MsgCategoryDomain.Error;
                    this.Msg = メッセージ.選択対象なし;
                    return this.Page();
                }

                // 既更新チェック
                if (goods.UpdatedAt.TruncMillSecond() != this.TargetGoods.UpdatedAt)
                {
                    this.MsgCategory = MsgCategoryDomain.Error;
                    this.Msg = メッセージ.既更新;
                    return this.Page();
                }
                var now = DateTime.UtcNow;
                goods.Stock = this.TargetGoods.Stock;
                goods.StockUpdatedAt = now;
                goods.UpdatedAt = now;
                goods.UpdatedBy = this.UserID;

                try
                {
                    await this.DBContext.SaveChangesAsync();
                    tran.Commit();
                    return new RedirectResult("/JamGoodsList");
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