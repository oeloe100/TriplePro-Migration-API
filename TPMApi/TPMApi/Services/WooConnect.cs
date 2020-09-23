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
        public static WCObject WcObject(string id, string secret)
        {
            RestAPI restApi = new RestAPI("https://www.hetsteigerhouthuis.nl/wp-json/wc/v3/",
                        id, secret, requestFilter: RequestFilter);

            WCObject wcObject = new WCObject(restApi);

            return wcObject;
        }

        private static void RequestFilter(HttpWebRequest request)
        {
            request.UserAgent = "Woocommerce.NET";
        }
    }
}
