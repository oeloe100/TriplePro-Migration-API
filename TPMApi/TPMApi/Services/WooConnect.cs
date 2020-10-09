using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WooCommerceNET;
using WooCommerceNET.WooCommerce.v3;

namespace TPMApi.Services
{
    public class WooConnect
    {
        /// <summary>
        /// Only Legacy WCObject supports product count.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static WooCommerceNET.WooCommerce.Legacy.WCObject LegacyWcObject(string id, string secret)
        { 
            RestAPI restApi = new RestAPI("https://www.hetsteigerhouthuis.nl/wc-api/v3/",
                        id, secret, requestFilter: RequestFilter);

            WooCommerceNET.WooCommerce.Legacy.WCObject wcObject = 
                new WooCommerceNET.WooCommerce.Legacy.WCObject(restApi);

            return wcObject;
        }

        /// <summary>
        /// Connect to WooCommerce Rest API and create/return WCObject
        /// </summary>
        /// <param name="id"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static WCObject WcObject(string id, string secret)
        {
            RestAPI restApi = new RestAPI("https://www.hetsteigerhouthuis.nl/wp-json/wc/v3/",
                        id, secret, requestFilter: RequestFilter, responseFilter: ResponseFilter);

            WCObject wcObject = new WCObject(restApi);

            return wcObject;
        }

        public static RestAPI WcRestAPI(string id, string secret)
        {
            RestAPI restApi = new RestAPI("https://www.hetsteigerhouthuis.nl/wp-json/wc/v3/",
                        id, secret, requestFilter: RequestFilter, responseFilter: ResponseFilter);

            return restApi;
        }

        /// <summary>
        /// Neccesary filter. (like scope)
        /// </summary>
        /// <param name="request"></param>
        private static void RequestFilter(HttpWebRequest request)
        {
            request.UserAgent = "Woocommerce.NET";
        }

        private static void ResponseFilter(HttpWebResponse response)
        {
            var total = int.Parse(response.Headers["X-WP-Total"]);
            var pagecount = int.Parse(response.Headers["X-WP-TotalPages"]);
        }
    }
}
