using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TPMApi.Models
{
    public class AuthorizationModel
    {
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string CallbackUrl { get; set; }
    }
}
