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
            List<string> combinations);

        List<Dictionary<string, string>> SortKeyValuePairByOrigin(List<string> combination);

        bool IsAny<T>(IEnumerable<T> data);

        Inventory SetCustomInventory(int total);

        List<Prices> SetCustomPrices(decimal? price);

        List<Options> BuildOptions(List<Dictionary<string, string>> dictList);

        void ItemPriceAdjustment(Items item);
    }
}