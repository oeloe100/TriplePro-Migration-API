using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TPMApi.Models
{
    public class AfostoAccessPoco
    {
        public string Name { get; set; }
        public string AfostoClientId { get; set; }
        public string AfostoClientSecret { get; set; }
        public string ServerUrl { get; set; } = "https://app.afosto.com/";
        public string RedirectUrl { get; set; } = "https://localhost:44338/token";
    }
}
