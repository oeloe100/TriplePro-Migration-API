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

        void SetCustomItems(
            IDictionary<string, List<string>> options,
            List<Items> itemsList,
            bool isWashing);

        void ItemPriceAdjustment(Items item);

        List<ProductAttributeLine> UnusedAttributes();

        decimal? FinishTypePriceRange(bool isWashing);
    }
}
