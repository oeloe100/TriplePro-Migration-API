using AutoMapper.Configuration;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TPMApi.Helpers;
using TPMApi.Models;

namespace TPMApi.Middelware
{
    public class AfostoAuthorizationMiddelware
    {
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
    }
}
