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
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TPMApi;Trusted_Connection=True;MultipleActiveResultSets=true");
        }
    }
}
