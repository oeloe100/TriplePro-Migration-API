using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TPMApi.Models
{
    public class TokenPoco
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
