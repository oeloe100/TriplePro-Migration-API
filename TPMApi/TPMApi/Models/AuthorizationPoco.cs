using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TPMApi.Models
{
    public class AuthorizationPoco
    {
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string CallbackUrl { get; set; }
        public string AppServerUrl { get; set; }
        public string ApiServerUrl { get; set; }
    }
}
