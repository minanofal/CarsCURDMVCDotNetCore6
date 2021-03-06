// <auto-generated />
using System;
using CarsCURDMVCDotNetCor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CarsCURDMVCDotNetCor.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("CarsCURDMVCDotNetCor.Models.CarModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Discription")
                        .IsRequired()
                        .HasMaxLength(2500)
                        .HasColumnType("nvarchar(2500)");

                    b.Property<string>("Faul")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Hight")
                        .HasColumnType("float");

                    b.Property<byte[]>("Image")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<int>("MakesCompaneyId")
                        .HasColumnType("int");

                    b.Property<string>("ModelName")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<int>("MotorCapacity")
                        .HasColumnType("int");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<double>("Tall")
                        .HasColumnType("float");

                    b.Property<double>("Width")
                        .HasColumnType("float");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("MakesCompaneyId");

                    b.ToTable("Cars");
                });

            modelBuilder.Entity("CarsCURDMVCDotNetCor.Models.CarsColor", b =>
                {
                    b.Property<string>("Color")
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<int>("CarModelId")
                        .HasColumnType("int");

                    b.HasKey("Color", "CarModelId");

                    b.HasIndex("CarModelId");

                    b.ToTable("carsColors");
                });

            modelBuilder.Entity("CarsCURDMVCDotNetCor.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<byte[]>("Image")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("CarsCURDMVCDotNetCor.Models.CategoryMakesCompaney", b =>
                {
                    b.Property<int>("MakesCompaneyId")
                        .HasColumnType("int");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.HasKey("MakesCompaneyId", "CategoryId");

                    b.HasIndex("CategoryId");

                    b.ToTable("CategoryMakesCompaney");
                });

            modelBuilder.Entity("CarsCURDMVCDotNetCor.Models.MakesCompaney", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<byte[]>("Image")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("Id");

                    b.ToTable("MakesCompaneys");
                });

            modelBuilder.Entity("CarsCURDMVCDotNetCor.Models.CarModel", b =>
                {
                    b.HasOne("CarsCURDMVCDotNetCor.Models.Category", "Category")
                        .WithMany("Models")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CarsCURDMVCDotNetCor.Models.MakesCompaney", "MakesCompaney")
                        .WithMany("Models")
                        .HasForeignKey("MakesCompaneyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("MakesCompaney");
                });

            modelBuilder.Entity("CarsCURDMVCDotNetCor.Models.CarsColor", b =>
                {
                    b.HasOne("CarsCURDMVCDotNetCor.Models.CarModel", "CarModel")
                        .WithMany("CarsColors")
                        .HasForeignKey("CarModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CarModel");
                });

            modelBuilder.Entity("CarsCURDMVCDotNetCor.Models.CategoryMakesCompaney", b =>
                {
                    b.HasOne("CarsCURDMVCDotNetCor.Models.Category", "Category")
                        .WithMany("CategoryMakesCompaneys")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CarsCURDMVCDotNetCor.Models.MakesCompaney", "MakesCompaney")
                        .WithMany("CategoryMakesCompaneys")
                        .HasForeignKey("MakesCompaneyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("MakesCompaney");
                });

            modelBuilder.Entity("CarsCURDMVCDotNetCor.Models.CarModel", b =>
                {
                    b.Navigation("CarsColors");
                });

            modelBuilder.Entity("CarsCURDMVCDotNetCor.Models.Category", b =>
                {
                    b.Navigation("CategoryMakesCompaneys");

                    b.Navigation("Models");
                });

            modelBuilder.Entity("CarsCURDMVCDotNetCor.Models.MakesCompaney", b =>
                {
                    b.Navigation("CategoryMakesCompaneys");

                    b.Navigation("Models");
                });
#pragma warning restore 612, 618
        }
    }
}
