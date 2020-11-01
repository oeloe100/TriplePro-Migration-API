using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TPMApi.Clients
{
    public class AfostoHttpClient
    {
        public HttpClient AfostoClient;
        private readonly string _accessToken;
        private readonly int _page;

        public AfostoHttpClient(string accessToken, int page)
        {
            _accessToken = accessToken;
            Init();
        }

        /// <summary>
        /// Our respectable HTTP Client. Has afosto required headers (pagesize, page, bearer + Accesstoken)
        /// </summary>
        private void Init()
        {
            string api = "https://localhost:5001/";

            AfostoClient = new HttpClient();
            AfostoClient.BaseAddress = new Uri(api);
            AfostoClient.DefaultRequestHeaders.Accept.Clear();
            
            if (!string.IsNullOrEmpty(_accessToken))
            {
                AfostoClient.DefaultRequestHeaders.Add("page", _page.ToString());
                AfostoClient.DefaultRequestHeaders.Add("pagesize", "50");
                AfostoClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            }

            AfostoClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
