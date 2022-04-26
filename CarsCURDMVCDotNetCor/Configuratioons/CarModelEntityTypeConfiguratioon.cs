using CarsCURDMVCDotNetCor.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarsCURDMVCDotNetCor.Configuratioons
{
    public class CarModelEntityTypeConfiguratioon : IEntityTypeConfiguration<CarModel>
    {
        public void Configure(EntityTypeBuilder<CarModel> builder)
        {


            builder.HasKey(x => new { x.ModelName, x.Year });
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ModelName).IsRequired().HasMaxLength(250);

            builder.Property(x => x.Year).IsRequired();
            builder.Property(x => x.Price).IsRequired();
            builder.Property(x => x.Hight).IsRequired();

            builder.Property(x => x.Faul).IsRequired();
            builder.Property(x => x.Tall).IsRequired();
            builder.Property(x => x.Width).IsRequired();
            builder.Property(x => x.Image).IsRequired();
            builder.Property(x => x.Discription).IsRequired();
            builder.Property(x=> x.MotorCapacity).IsRequired();

            builder.Property(x => x.Discription).HasMaxLength(2500);

            builder.HasOne(x=>x.Category)
                .WithMany(C=>C.Models)
                .HasPrincipalKey(C => C.Id)
                .HasForeignKey(x=>x.CategoryId);

            builder.HasOne(x => x.MakesCompaney)
                .WithMany(M => M.Models)
                .HasPrincipalKey(M => M.Id)
                .HasForeignKey(x => x.MakesCompaneyId);

            builder.HasMany(x => x.CarsColors)
                .WithOne(C => C.CarModel)
                .HasPrincipalKey(x => x.Id)
                .HasForeignKey(C => C.CarModelId);
        }
    }
}
