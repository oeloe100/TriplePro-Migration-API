using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TPMApi.Clients;
using TPMApi.Controllers;
using TPMApi.Models;

namespace TPMApi.Middelware
{
    public class MigrationMiddelware
    {
        private static int? _index;
        private static int? _currentProductId;

        /// <summary>
        /// Here we build the mapping product(s) model and post to afosto /products endpoint
        /// </summary>
        /// <param name="productsMapped"></param>
        /// <param name="config"></param>
        /// <param name="afostoAuthenticationData"></param>
        /// <returns></returns>
        public static async Task BuildWTAMappingModel(
            string accessToken,
            JObject productsMapped,
            IOptions<AuthorizationPoco> config,
            ILogger<MigrationController> logger,
            int index,
            int? productId)
        {
            _index = index;
            _currentProductId = productId;

            await PostAfostoProductModel("/products", config, logger, accessToken, productsMapped);
        }

        /// <summary>
        /// Post product model/data to afosto /products endpoint
        /// </summary>
        /// <param name="path"></param>
        /// <param name="config"></param>
        /// <param name="afostoAccessPoco"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static async Task PostAfostoProductModel(
            string path,
            IOptions<AuthorizationPoco> config,
            ILogger<MigrationController> logger,
            string accessToken,
            JObject data)
        {
            var apiClient = new AfostoHttpClient(accessToken);
            var requestUriString = string.Format("{0}{1}", config.Value.ApiServerUrl, path);
            var content = new StringContent(JsonConvert.SerializeObject(data));

            try
            {
                var result = await apiClient.AfostoClient.PostAsync(requestUriString, content);
                if (result.IsSuccessStatusCode)
                {
                    logger.LogInformation("Migration Success for id: " + _currentProductId);
                }
                else 
                {
                    logger.LogWarning("Migration Problem for id: " + _currentProductId);
                }
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex.Message);
                logger.LogCritical(ex.StackTrace);
            }

            logger.LogInformation("Current index: " + _index);
        }

        /// <summary>
        /// Get Metadata from afosto Metadata endpoint.
        /// necessary for buidling a allowed product model.
        /// </summary>
        /// <param name="afostoAccessPoco"></param>
        /// <param name="config"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<string> GetAfostoData(
            string accessToken,
            IOptions<AuthorizationPoco> config,
            string path)
        {
            var apiClient = new AfostoHttpClient(accessToken);
            var requestUriString = string.Format("{0}{1}", config.Value.ApiServerUrl, path);

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
