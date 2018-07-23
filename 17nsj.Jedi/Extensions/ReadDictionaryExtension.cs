using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace _17nsj.Jedi.Extensions
{
    public static class ReadDictionaryExtension
    {
        public static List<SelectListItem> ToSelectListItems(this ReadOnlyDictionary<string, string> dic)
        {
            var items = new List<SelectListItem>();

            foreach (var item in dic)
            {
                items.Add(new SelectListItem() { Text = item.Key, Value = item.Value.ToString() });
            }

            return items;
        }

        public static List<SelectListItem> ToSelectListItems(this ReadOnlyDictionary<string, int> dic)
        {
            var items = new List<SelectListItem>();

            foreach (var item in dic)
            {
                items.Add(new SelectListItem() { Text = item.Key, Value = item.Value.ToString() });
            }

            return items;
        }
    }
}
