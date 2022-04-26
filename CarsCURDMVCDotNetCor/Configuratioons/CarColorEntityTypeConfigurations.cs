using CarsCURDMVCDotNetCor.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarsCURDMVCDotNetCor.Configuratioons
{
    public class CarColorEntityTypeConfigurations : IEntityTypeConfiguration<CarsColor>
    {
        public void Configure(EntityTypeBuilder<CarsColor> builder)
        {
            builder.HasKey(x => new { x.Color, x.CarModelId });
            
            builder.Property(x => x.Color).IsRequired().HasMaxLength(250);
        }
    }
}
