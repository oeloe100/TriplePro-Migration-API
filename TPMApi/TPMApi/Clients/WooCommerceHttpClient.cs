using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TPMApi.Clients
{
    public class WooCommerceHttpClient
    {
        public HttpClient WooClient;
        private readonly string _AccessToken;

        public WooCommerceHttpClient()//string accessToken)
        {
            //_AccessToken = accessToken;
            Init();
        }

        /// <summary>
        /// Our respectable HTTP Client. Has woocommerce required headers (bearer + Accesstoken)
        /// </summary>
        private void Init()
        {
            string api = "https://localhost:5001/";

            WooClient = new HttpClient();
            WooClient.BaseAddress = new Uri(api);
            WooClient.DefaultRequestHeaders.Accept.Clear();
            WooClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "Y2tfYWNkZGNhYjg4N2NhMTM5NmE5NTgwYmJhOGRiOWY1NjlkODdmMDg5Mjpjc19lNGY5M2E3NDgxMDk3NjM3MWU2MDk3YmIwZjJkYzMyODZlMThiZDVh");
            WooClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
