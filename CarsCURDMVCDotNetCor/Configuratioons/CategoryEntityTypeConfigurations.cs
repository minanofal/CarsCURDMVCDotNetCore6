using CarsCURDMVCDotNetCor.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarsCURDMVCDotNetCor.Configuratioons
{
    public class CategoryEntityTypeConfigurations :IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(256);

            builder.Property(x => x.Image).IsRequired();

            builder.HasMany(x => x.MakesCompaneys)
                .WithMany(c => c.Categories).UsingEntity<CategoryMakesCompaney>(
                j=>j
                .HasOne(c=> c.MakesCompaney)
                .WithMany(M=>M.CategoryMakesCompaneys)
                .HasForeignKey(c=>c.MakesCompaneyId),
                j=>j
                .HasOne(c=>c.Category)
                .WithMany(C=>C.CategoryMakesCompaneys)
                .HasForeignKey(c=>c.CategoryId),
                j=>j
                .HasKey(c => new {c.MakesCompaneyId,c.CategoryId})
               );
            
        }
    }
}
