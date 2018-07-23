using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _17nsj.Jedi.Models
{
    public class JamGoodsByCategoryModel
    {
        public string CategoryName { get; set; }
        public List<JamGoodsModel> Goods { set; get; }
    }
}
