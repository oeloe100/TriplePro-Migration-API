﻿using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TPMApi.Builder.Afosto.Requirements;
using TPMApi.Customs.SteigerhouthuisCustom;
using TPMHelper.AfostoHelper.ProductModel;
using WooCommerceNET.WooCommerce.v3;

namespace TPMApi.Builder.Afosto
{
    public interface IAfostoProductBuilder : IAfostoWCRequirements
    {
        List<Images> SetImages();
        List<Collections> SetCollections();
        List<Descriptors> SetDescriptors();
        Inventory SetInventory(Variation variation);
        Task<List<Items>> SetItems();
        List<Options> SetOptions(Variation variation);
        List<Prices> SetPrices(Variation variation);
        Seo SetSeo();
        List<Specifications> SetSpecifications();
        Task<List<ProductCategory>> WooCategories();
        Task<List<Variation>> WooProdVariations();
        bool HasActiveCustoms(
            List<Items> items, 
            List<Variation> variations);
    }
}