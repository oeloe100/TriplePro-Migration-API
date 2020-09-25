using System;

namespace TPMDataLibrary.Models
{
    public class AfostoAccessModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string AfostoKey { get; set; }
        public string AfostoSecret { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int? ExpiresIn { get; set; }
        public DateTime Created_At { get; set; }
    }
}
