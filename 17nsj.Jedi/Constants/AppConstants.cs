using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace _17nsj.Jedi.Constants
{
    public class AppConstants
    {
        public static readonly ReadOnlyCollection<string> UndeliteableUsers = new ReadOnlyCollection<string>(new List<string>()
        {
            "user",
            "admin"
        });

        public static readonly int ExpireTimeMin = 1;
    }
}
