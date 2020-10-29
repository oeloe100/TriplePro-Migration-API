using System.Collections.Generic;
using TPMHelper.AfostoHelper.ProductModel;
using WooCommerceNET.WooCommerce.v3;

namespace TPMApi.Customs.SteigerhouthuisCustom
{
    public interface ISteigerhoutCustomOptionsBuilder
    {
        void BuildCustomOptions(
            List<Items> items,
            List<Variation> variations);

        void FillComboList(List<List<string>> CombinationsList);

        void SetCustomItems(
            List<Items> itemsList,
            List<List<string>> result,
            List<Variation> variations);

        Inventory SetCustomInventory(int total);

        List<Options> BuildOptions(List<Dictionary<string, string>> dictList);

        List<Prices> SetCustomPrices(decimal? price);

        /*void ItemPriceAdjustment(Items item);*/
    }
}