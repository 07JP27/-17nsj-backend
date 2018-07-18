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
        public const string 庶務 = "1";
        public const string プログラム = "2";
        public const string 報道 = "3";
        public const string 企業連携 = "4";
        public const string CPT = "5";
        public const string DPT = "6";
        public const string 副部長 = "7";
        public const string 部長 = "8";

        public static readonly ReadOnlyDictionary<string, string> DomainList = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>()
        {
            { GetName(不明), 不明 },
            { GetName(庶務), 庶務 },
            { GetName(プログラム), プログラム },
            { GetName(報道), 報道 },
            { GetName(企業連携), 企業連携 },
            { GetName(CPT), CPT },
            { GetName(DPT), DPT },
            { GetName(副部長), 副部長 },
            { GetName(部長), 部長 }
        });

        public static string GetName(string value)
        {
            switch (value)
            {
                case 不明:
                    return "不明";
                case 庶務:
                    return "庶務";
                case プログラム:
                    return "プログラム";
                case 報道:
                    return "報道";
                case 企業連携:
                    return "企業連携";
                case CPT:
                    return "CPT";
                case DPT:
                    return "DPT";
                case 副部長:
                    return "副部長";
                case 部長:
                    return "部長";
                default:
                    return string.Empty;
            }
        }
    }
}
