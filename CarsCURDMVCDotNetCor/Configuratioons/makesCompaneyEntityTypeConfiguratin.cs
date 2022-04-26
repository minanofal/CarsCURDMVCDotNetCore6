using CarsCURDMVCDotNetCor.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarsCURDMVCDotNetCor.Configuratioons
{
    public class makesCompaneyEntityTypeConfiguratin :IEntityTypeConfiguration<MakesCompaney>
    {
       public void Configure(EntityTypeBuilder<MakesCompaney> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x=>x.Name).IsRequired().HasMaxLength(250);

            builder.Property(x => x.Image).IsRequired();

        }
    }
}
