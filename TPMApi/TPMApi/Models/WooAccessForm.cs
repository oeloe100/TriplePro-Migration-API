using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TPMApi.Models
{
    public class WooAccessForm
    {
        public string ClientSecret { get; set; }
        public string ClientKey { get; set; }
        public string CallbackUrl { get; set; }        
    }
}
