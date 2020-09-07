using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TPMApi.TokenProvider
{
    public class TokenOptions
    {
        public string Path { get; set; } = "/token";
        public string Issuer { get; set; }
        public string Audiance { get; set; }
        public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(5);
        public SigningCredentials SigningCredentials { get; set; }
        public bool ValidateIssuerSigningKey { get; set; }
        public bool ValidateIssuer { get; set; }
        public X509SecurityKey IssuerSigningKey { get; set; }
    }
}
