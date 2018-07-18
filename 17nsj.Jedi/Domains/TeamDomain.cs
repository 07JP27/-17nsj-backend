using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace _17nsj.Jedi.Domains
{
    public class TeamDomain
    {
        public const string 不明 = "0";
        public const string 部長 = "1";
        public const string 副部長 = "2";
        public const string DPT = "3";
        public const string CPT = "4";

        public static readonly ReadOnlyDictionary<string, string> DomainList = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>()
        {
            { GetName(不明), 不明 },
            { GetName(部長), 部長 },
            { GetName(副部長), 副部長 },
            { GetName(DPT), DPT },
            { GetName(CPT), CPT }
        });

        public static string GetName(string value)
        {
            switch (value)
            {
                case 部長:
                    return "部長";
                case 副部長:
                    return "副部長";
                case DPT:
                    return "DPT";
                case CPT:
                    return "CPT";
                default:
                    return string.Empty;
            }
        }
    }
}
