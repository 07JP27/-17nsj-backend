using _17nsj.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace _17nsj.Repository
{
    public class JediDbContext : DbContext
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="options">コンテキストオプション</param>
        public JediDbContext(DbContextOptions<JediDbContext> options)
            : base(options)
        {
            // 処理なし
        }

        public virtual DbSet<ActivityCategories> ActivityCategories { get; set; }
        public virtual DbSet<Movies> Movies { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<NewsCategories> NewsCategories { get; set; }
        public virtual DbSet<Newspapers> Newspapers { get; set; }
        public virtual DbSet<MobileAppConfig> MobileAppConfig { get; set; }
        public virtual DbSet<Activities> Activities { get; set; }
        public virtual DbSet<Documents> Documents { get; set; }
        public virtual DbSet<Schedules> Schedules { get; set; }
        public virtual DbSet<JamGoods> JamGoods { get; set; }
        public virtual DbSet<JamGoodsCategories> JamGoodsCategories { get; set; }
    }
}
