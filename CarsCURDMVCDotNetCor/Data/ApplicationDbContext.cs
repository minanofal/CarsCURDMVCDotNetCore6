using CarsCURDMVCDotNetCor.Configuratioons;
using CarsCURDMVCDotNetCor.Models;
using Microsoft.EntityFrameworkCore;

namespace CarsCURDMVCDotNetCor.Data
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new CarModelEntityTypeConfiguratioon().Configure(modelBuilder.Entity<CarModel>());
            new CategoryEntityTypeConfigurations().Configure(modelBuilder.Entity<Category>());
            new makesCompaneyEntityTypeConfiguratin().Configure(modelBuilder.Entity<MakesCompaney>());
            new CarColorEntityTypeConfigurations().Configure(modelBuilder.Entity<CarsColor>());
        }

        public DbSet<CarModel> Cars { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<MakesCompaney> MakesCompaneys { get; set; }

        public DbSet<CarsColor> carsColors { get; set; }

        public DbSet<CategoryMakesCompaney> CategoryMakesCompaney { get; set; }

    }
}
