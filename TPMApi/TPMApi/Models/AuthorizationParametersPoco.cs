using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TPMApi.Models
{
    public class AuthorizationParametersPoco
    {
        public string ServerUrl { get; set; }
        public string Code { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUrl { get; set; }
        public string GrantType { get; set; }
    }
}
