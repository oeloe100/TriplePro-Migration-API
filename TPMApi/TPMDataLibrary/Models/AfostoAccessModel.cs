using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TPMDataLibrary.Models
{
    public class AfostoAccessModel
    {
        public int Id { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int? ExpiresIn { get; set; }
    }
}
