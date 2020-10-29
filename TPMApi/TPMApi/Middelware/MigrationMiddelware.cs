using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using TPMApi.Clients;
using TPMApi.Controllers;
using TPMApi.Models;
using WooCommerceNET;
using WooCommerceNET.WooCommerce.v3;
using MediaTypeHeaderValue = System.Net.Http.Headers.MediaTypeHeaderValue;

namespace TPMApi.Middelware
{
    public class MigrationMiddelware
    {
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
            int? productId)
        {
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
                HttpResponseMessage result = await apiClient.AfostoClient.PostAsync(requestUriString, content);
                if (!result.IsSuccessStatusCode)
                {
                    string body = await result.Content.ReadAsStringAsync();
                    logger.LogInformation("Fail with ID: " + _currentProductId);

                    logger.LogError(body);
                }
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex.Message);
                logger.LogCritical(ex.StackTrace);
            }

            logger.LogInformation("Current: " + _currentProductId + " > Next > ");
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

        public static async Task<List<AfostoImageModelAfterUpload>> UploadImageToAfosto(
            string accessToken,
            RestAPI wcRestAPI,
            IWebHostEnvironment env,
            List<ProductImage> imageObjectList,
            Dictionary<string, string> parms = null)
        {
            var apiClient = new AfostoHttpClient(accessToken);
            var url = "https://upload.afosto.com/v2/product";
            var testLocation = "https://overclothing.com/wp-content/uploads/sites/3/2020/09/night-on-fire-mens-white-a-3-600x600.jpg";
            var test = "https://hetsteigerhouthuis.nl/wp-content/uploads/2020/04/IMG_4896.jpg";

            List<AfostoImageModelAfterUpload> imageList = new List<AfostoImageModelAfterUpload>();

            using (var httpClient = apiClient.AfostoClient)
            {
                using (var form = new MultipartFormDataContent())
                {
                    var testRun = httpClient.GetAsync(imageObjectList[0].src).Result;

                    if (testRun.IsSuccessStatusCode)
                    { 
                        foreach (var imageObject in imageObjectList)
                        { 
                            byte[] imageBytes = await httpClient.GetByteArrayAsync(imageObject.src);

                            using (var fileContent = new ByteArrayContent(imageBytes))
                            {
                                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

                                form.Add(fileContent, "file", Path.GetFileName(imageObject.src));
                                HttpResponseMessage response = httpClient.PostAsync(url, form).Result;

                                if (response.IsSuccessStatusCode)
                                {
                                    string body = await response.Content.ReadAsStringAsync();
                                    var customModel = JsonConvert.DeserializeObject<AfostoImageModelAfterUpload>(body);

                                    imageList.Add(customModel);
                                }
                            }
                        }
                    }
                }
            }

            return imageList;
        }
    }
}
