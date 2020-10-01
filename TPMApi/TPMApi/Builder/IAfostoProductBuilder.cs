using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TPMApi.Interfaces;
using TPMHelper.AfostoHelper.ProductModel;
using WooCommerceNET.WooCommerce.v3;

namespace TPMApi.Helpers
{
    public interface IAfostoProductBuilder : IWCRequirements
    {
        string EanCheck(string sourceEan);
        string Rdm();
        JArray SetCollections();
        List<Descriptors> SetDescriptors();
        Inventory SetInventory(Variation variation);
        Task<List<Items>> SetItems();
        List<Options> SetOptions(Variation variation);
        List<Prices> SetPrices(Variation variation);
        Seo SetSeo();
        List<Specifications> SetSpecifications();
        string SkuGenerator(int? id);
        Task<List<ProductCategory>> WooCategories();
        Task<List<Variation>> WooProdVariations();
    }
}