using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _17nsj.Jedi.Models
{
    public class NewsModel
    {
        public string Category { get; set; }
        public string CategoryStr { get; set; }
        public string CategoryColor { get; set; }
        public int Id { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Outline { get; set; }
        public string MediaURL { get; set; }
        public string RelationalURL { get; set; }
        public string ThumbnailURL { get; set; }
        public bool IsAvailable { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
    }
}
