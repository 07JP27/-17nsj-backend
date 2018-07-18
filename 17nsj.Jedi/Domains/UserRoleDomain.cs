using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _17nsj.Jedi.Domains
{
    public class UserRoleDomain
    {
        public const string Reader = "0";
        public const string Writer = "1";
        public const string Admin = "2";
        public const string SysAdmin = "3";

        public static string GetName(string value)
        {
            switch (value)
            {
                case Reader:
                    return "読み取りユーザー";
                case Writer:
                    return "一般ユーザー";
                case Admin:
                    return "管理者";
                case SysAdmin:
                    return "システム管理者";
                default:
                    return string.Empty;
            }
        }
    }
}
