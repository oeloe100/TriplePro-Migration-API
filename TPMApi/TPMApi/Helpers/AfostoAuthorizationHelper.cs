using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TPMApi.Clients;
using TPMApi.Middelware;

namespace TPMApi.Helpers
{
    public class AfostoAuthorizationHelper
    {
        private AfostoHttpClient _afostoHttpClient;

        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _serverUrl;

        public AfostoAuthorizationHelper(
            string clientId, 
            string clientSecret, 
            string serverUrl)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _serverUrl = serverUrl;

            //no need to pass token to client from this constructor
            _afostoHttpClient = new AfostoHttpClient(null);
        }

        public async Task<string> AuthorizeClient(string code, string grantType, string redirectUrl)
        {
            string requestUriString = string.Format("{0}/api/token", _serverUrl);

            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("client_secret", _clientSecret),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("grant_type", grantType),
                new KeyValuePair<string, string>("redirect_uri", redirectUrl)
            });

            using (HttpResponseMessage resp = await _afostoHttpClient.AfostoClient.PostAsync(requestUriString, data))
            {
                if (resp.IsSuccessStatusCode)
                {
                    var jsonResult = await resp.Content.ReadAsStringAsync();
                    return jsonResult;
                }

                throw new Exception(resp.ReasonPhrase);
            }
        }
    }
}
