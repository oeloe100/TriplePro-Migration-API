using System.Collections.Generic;
using TPMHelper.AfostoHelper.ProductModel;
using WooCommerceNET.WooCommerce.v3;

namespace TPMApi.Customs.SteigerhouthuisCustom
{
    public interface ISteigerhoutCustomOptionsBuilder
    {
        void BuildCustomOptions(
            List<Items> items,
            List<Variation> variations,
            string washingTitle,
            IDictionary<string, bool> bundledAccessManger);

        void FillComboList(List<List<string>> CombinationsList);

        void SetCustomItems(
            List<Items> itemsList,
            List<List<string>> result,
            List<Variation> variations,
            string washingTitle,
            IDictionary<string, bool> bundledAccessManger);

        Inventory SetCustomInventory(int total);

        List<Options> BuildOptions(List<Dictionary<string, string>> dictList);

        List<Prices> SetCustomPrices(decimal? price);

        /*void ItemPriceAdjustment(Items item);*/
    }
}