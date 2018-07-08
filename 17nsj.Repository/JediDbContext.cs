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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Users>();
            entity.ToTable("MST_Users", "dbo").HasKey(p => p.UserId);

            modelBuilder.Entity<Activities>().HasKey(c => new { c.Category, c.Id });
            modelBuilder.Entity<JamGoods>().HasKey(c => new { c.Category, c.Id });
            modelBuilder.Entity<News>().HasKey(c => new { c.Category, c.Id });
            modelBuilder.Entity<Schedules>().HasKey(c => new { c.Title, c.HasRange, c.Start });
            modelBuilder.Entity<MobileAppConfig>().HasKey(c => new { c.ForceUpdate, c.DroidVersion, c.DroidStoreURL, c.iOSVersion, c.iOSStoreURL });
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
