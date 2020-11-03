using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TPMApi.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

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
