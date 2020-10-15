using System.Collections.Generic;

namespace TPMApi.Customs.SteigerhouthuisCustom
{
    public interface ISortKVP<T>
    {
        List<Dictionary<string, string>> SortKeyValuePairByOrigin(T combination);
    }
}
