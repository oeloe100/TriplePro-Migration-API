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
        private readonly string _AccessToken;

        public AfostoHttpClient(string accessToken)
        {
            _AccessToken = accessToken;
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
            
            if (!string.IsNullOrEmpty(_AccessToken))
            {
                AfostoClient.DefaultRequestHeaders.Add("page", "1");
                AfostoClient.DefaultRequestHeaders.Add("pagesize", "50");
                AfostoClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _AccessToken);
            }

            AfostoClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
