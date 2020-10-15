using System.Collections.Generic;
using WooCommerceNET.WooCommerce.v3;

namespace TPMApi.Customs.SteigerhouthuisCustom
{
    public interface IPriceCalculator<T>
    {
        decimal? Price(
            List<Dictionary<string, string>> optionList,
            List<Variation> variations);
    }
}
