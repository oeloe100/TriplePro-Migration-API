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

        private void Init()
        {
            string api = "https://localhost:44338/";

            AfostoClient = new HttpClient();
            AfostoClient.BaseAddress = new Uri(api);
            AfostoClient.DefaultRequestHeaders.Accept.Clear();
            if (!string.IsNullOrEmpty(_AccessToken))
                AfostoClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _AccessToken);
            AfostoClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
        }
    }
}
