using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TPMApi.Clients;
using TPMApi.Mapping.WTAMapping;
using TPMApi.Models;

namespace TPMApi.Middelware
{
    public class MigrationMiddelware
    {
        /// <summary>
        /// Here we build the mapping product(s) model and post to afosto /products endpoint
        /// </summary>
        /// <param name="productsMapped"></param>
        /// <param name="config"></param>
        /// <param name="afostoAuthenticationData"></param>
        /// <returns></returns>
        public static async Task BuildWTAMappingModel(
            List<JObject> productsMapped, 
            IOptions<AuthorizationPoco> config,
            AfostoTokensPoco afostoAuthenticationData)
        {
            await Post("/products", config, afostoAuthenticationData, productsMapped);
        }

        /// <summary>
        /// Post product model/data to afosto /products endpoint
        /// </summary>
        /// <param name="path"></param>
        /// <param name="config"></param>
        /// <param name="afostoAccessPoco"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static async Task<object> Post(
            string path, 
            IOptions<AuthorizationPoco> config,
            AfostoTokensPoco afostoAccessPoco,
            List<JObject> data)
        {
            var apiClient = new AfostoHttpClient(afostoAccessPoco.AccessToken);
            var requestUriString = string.Format("{0}{1}", config.Value.ApiServerUrl, path);

            var dataAsJsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(dataAsJsonString);

            using (apiClient.AfostoClient)
            {
                var result = await apiClient.AfostoClient.PostAsync(requestUriString, content);

                if (result.IsSuccessStatusCode)
                {
                    string resultContent = await result.Content.ReadAsStringAsync();
                    return resultContent;
                }

                throw new Exception(result.ReasonPhrase);
            }
        }

        /// <summary>
        /// Get Metadata from afosto Metadata endpoint.
        /// necessary for buidling a allowed product model.
        /// </summary>
        /// <param name="afostoAccessPoco"></param>
        /// <param name="config"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<string> GetAfostoMetaData(
            AfostoTokensPoco afostoAccessPoco,
            IOptions<AuthorizationPoco> config,
            string path)
        {
            var apiClient = new AfostoHttpClient(afostoAccessPoco.AccessToken);
            var requestUriString = string.Format("{0}{1}", config.Value.ApiServerUrl, path);

            using (apiClient.AfostoClient)
            {
                var result = await apiClient.AfostoClient.GetAsync(requestUriString);

                if (result.IsSuccessStatusCode)
                {
                    string resultContent = await result.Content.ReadAsStringAsync();
                    return resultContent;
                }

                throw new Exception(result.ReasonPhrase);
            }
        }
    }
}
