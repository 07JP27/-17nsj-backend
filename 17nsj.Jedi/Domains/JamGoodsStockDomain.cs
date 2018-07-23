using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace _17nsj.Jedi.Domains
{
    public class JamGoodsStockDomain
    {
        public const int 在庫情報なし = 0;
        public const int 在庫あり = 1;
        public const int 在庫わずか = 2;
        public const int 売り切れ = 3;

        public static readonly ReadOnlyDictionary<string, int> DomainList = new ReadOnlyDictionary<string, int>(new Dictionary<string, int>()
        {
            { GetName(在庫情報なし), 在庫情報なし },
            { GetName(在庫あり), 在庫あり },
            { GetName(在庫わずか), 在庫わずか },
            { GetName(売り切れ), 売り切れ }
        });

        public static string GetName(int value)
        {
            switch (value)
            {
                case 在庫情報なし:
                    return nameof(在庫情報なし);
                case 在庫あり:
                    return nameof(在庫あり);
                case 在庫わずか:
                    return nameof(在庫わずか);
                case 売り切れ:
                    return nameof(売り切れ);
                default:
                    return string.Empty;
            }
        }
    }
}
