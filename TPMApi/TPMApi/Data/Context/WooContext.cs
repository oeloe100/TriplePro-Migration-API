using Microsoft.EntityFrameworkCore;
using TPMApi.Models;

namespace TPMApi.Data.Context
{
    public class WooContext : DbContext
    {
        public DbSet<WooAccessModel> WooAccess { get; set; }

        /// <summary>
        /// Custom context this creates a database using code first principal.
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=sql.triplepromigrationapi.nl;Database=triplepromigrationap;User ID=triplepromigrationap;Password=s(AQQ26Jjm;");
        }
    }
}
