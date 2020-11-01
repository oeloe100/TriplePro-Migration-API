using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TPMApi.Clients;

namespace TPMApi.Middelware
{
    public static class AfostoAuthorizationMiddelware
    {
        //private static AfostoHttpClient _afostoHttpClient;

        /// <summary>
        /// Build afosto authorizaton Url
        /// </summary>
        /// <param name="serverUrl"></param>
        /// <param name="clientId"></param>
        /// <param name="callbackUrl"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static string RequestAuthorizationUrl(
            string serverUrl,
            string clientId,
            string callbackUrl,
            string state = null)
        {
            string callbackUri = new Uri(callbackUrl).ToString();
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("{0}/oauth/authorize", serverUrl);
            stringBuilder.AppendFormat("?client_id={0}", HttpUtility.UrlEncode(clientId));
            stringBuilder.AppendFormat("&redirect_uri={0}", HttpUtility.UrlEncode(callbackUri));
            stringBuilder.Append("&response_type=code");
            stringBuilder.Append("&scope=all");

            if (!string.IsNullOrEmpty(state))
            {
                stringBuilder.AppendFormat("&state={0}", state);
            }
            else
            {
                return null;
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Authorize with token endpoint and receive accesstoken + refresh etc..
        /// </summary>
        /// <param name="serverUrl"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="redirectUrl"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static async Task<string> AuthorizeClient(
            string serverUrl,
            string clientId,
            string clientSecret,
            string redirectUrl,
            string code)
        {
            //instantiate with empty string. No Accesstoken to insert.
            var afostoHttpClient = new AfostoHttpClient(null, 1);

            string requestUriString = string.Format("{0}/oauth/token", serverUrl);

            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("redirect_uri", redirectUrl)
            });

            using (HttpResponseMessage resp = await afostoHttpClient.AfostoClient.PostAsync(requestUriString, data))
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
