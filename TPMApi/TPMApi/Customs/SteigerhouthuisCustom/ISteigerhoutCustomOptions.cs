using System.Collections.Generic;
using TPMHelper.AfostoHelper.ProductModel;
using WooCommerceNET.WooCommerce.v3;

namespace TPMApi.Customs.SteigerhouthuisCustom
{
    public interface ISteigerhoutCustomOptions
    {
        List<Items> BuildCustomOptions();

        void SetCustomItems(
            IDictionary<string, List<string>> options,
            List<Items> itemsList,
            bool isWashing);

        void ItemPriceAdjustment(Items item);

        List<ProductAttributeLine> UnusedAttributes();

        decimal? FinishTypePriceRange(bool isWashing);

        /*void BuildVariantsOfUnusedMarkedAttributes(
            List<Items> items,
            Variation variation);*/

        void BuildNewWashings(List<Items> items);
    }
}
