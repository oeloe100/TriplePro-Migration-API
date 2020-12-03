using Microsoft.EntityFrameworkCore;
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
        }
    }
}
