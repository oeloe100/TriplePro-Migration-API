using System;

namespace TPMApi.Models
{
    public class WooAccessModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string WooClientKey { get; set; }
        public string WooClientSecret { get; set; }
        public DateTime Created_At { get; set; }
    }
}
