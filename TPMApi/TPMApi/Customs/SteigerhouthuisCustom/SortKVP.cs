using System;
using System.Collections.Generic;
using System.Linq;
using WooCommerceNET.WooCommerce.v3;

namespace TPMApi.Customs.SteigerhouthuisCustom
{
    public class SortKVP : ISortKVP<List<string>>
    {
        private List<Dictionary<string, string>> _optionsList;
        private readonly Product Product;

        public SortKVP(Product product)
        {
            Product = product;
        }

        public List<Dictionary<string, string>> SortKeyValuePairByOrigin(List<string> combination)
        {
            _optionsList = new List<Dictionary<string, string>>();

            foreach (var option in combination)
            {
                if (_optionsList.Count < combination.Count)
                {
                    //CustomKVPSorted(option);
                    ExistingKVPSorted(option);
                }
            }

            return _optionsList;
        }

        private void ExistingKVPSorted(string option)
        {
            for (var i = 0; i < Product.attributes.Count; i++)
            {
                if (Product.attributes[i].variation == true)
                {
                    var attribute = Product.attributes[i].options.Where(a => a == option);
                    if (IsAny(attribute))
                    {
                        ManageCustomOptionsList(
                            Product.attributes[i].name,
                            option);

                        return;
                    }
                }
            }
        }

        /*private void CustomKVPSorted(string option)
        {
            var coatingChallange = SteigerhoutOptionsData.CoationgOptions().Where(a => a == option);
            if (IsAny(coatingChallange))
            {
                if (_optionsList[_optionsList.Count - 1].ContainsKey("coating") == false)
                { 
                    ManageCustomOptionsList("coating", option);
                    return;
                }
            }

            var washingChallange = SteigerhoutOptionsData.WashingOptions().Where(a => a == option);
            if (IsAny(washingChallange))
            {
                ManageCustomOptionsList("washing", option);
                return;
            }
        }*/

        private bool IsAny<T>(IEnumerable<T> data)
        {
            return data != null && data.Any();
        }

        private void ManageCustomOptionsList(
            string type,
            string option)
        {
            var dict = new Dictionary<string, string>
            {
                { type, option }
            };

            _optionsList.Add(dict);
        }
    }
}
