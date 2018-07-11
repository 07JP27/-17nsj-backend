using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _17nsj.Jedi.Models
{
    public class JamGoodsModel
    {
        public string Category { get; set; }
        public string CategoryName { get; set; }
        public int Id { get; set; }
        public int DisplayOrder { get; set; }
        public string ThumbnailURL { get; set; }
        public string DetailImageURL { get; set; }
        public int Stock { get; set; }
        public System.DateTime StockUpdatedAt { get; set; }
        public string PartsNumber { get; set; }
        public string GoodsName { get; set; }
        public int Price { get; set; }
        public string Size { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
    }
}
