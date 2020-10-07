using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TPMHelper.AfostoHelper.ProductModel;
using WooCommerceNET.WooCommerce.v3;

namespace TPMApi.Builder.Afosto.SteigerhouthuisCustom
{
    interface ISteigerhoutCustomOptions
    {
        List<Items> BuildCustomOptions();

        List<Items> SetCustomItems(IDictionary<string, List<string>> options, bool isWashing);

        void ItemPriceAdjustment(Items item);

        List<ProductAttributeLine> UnusedAttributes();

        decimal? FinishTypePriceRange(bool isWashing);
    }
}
