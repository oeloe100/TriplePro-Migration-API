using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TPMDataLibrary.Models;

namespace TPMApi.Data.Context
{
    public class AfostoContext : DbContext
    {
        public DbSet<AfostoAccessModel> AfostoAccess { get; set; }

        /// <summary>
        /// Custom context this creates a database using code first principal.
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=sql.triplepromigrationapi.nl;Database=triplepromigrationap;User ID=triplepromigrationap;Password=s(AQQ26Jjm;");
                //"Data Source=sql.triplepromigrationapi.nl;Initial Catalog=triplepromigrationap;User ID=triplepromigrationap;Password=s(AQQ26Jjm;");
        }
    }
}
