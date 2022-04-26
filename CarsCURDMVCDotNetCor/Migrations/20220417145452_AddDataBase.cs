using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarsCURDMVCDotNetCor.Migrations
{
    public partial class AddDataBase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MakesCompaneys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MakesCompaneys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModelName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Hight = table.Column<double>(type: "float", nullable: false),
                    Tall = table.Column<double>(type: "float", nullable: false),
                    Width = table.Column<double>(type: "float", nullable: false),
                    Faul = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Discription = table.Column<string>(type: "nvarchar(2500)", maxLength: 2500, nullable: false),
                    MotorCapacity = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    MakesCompaneyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cars_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cars_MakesCompaneys_MakesCompaneyId",
                        column: x => x.MakesCompaneyId,
                        principalTable: "MakesCompaneys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CategoryMakesCompaney",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    MakesCompaneyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryMakesCompaney", x => new { x.MakesCompaneyId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_CategoryMakesCompaney_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryMakesCompaney_MakesCompaneys_MakesCompaneyId",
                        column: x => x.MakesCompaneyId,
                        principalTable: "MakesCompaneys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "carsColors",
                columns: table => new
                {
                    Color = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CarModelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_carsColors", x => new { x.Color, x.CarModelId });
                    table.ForeignKey(
                        name: "FK_carsColors_Cars_CarModelId",
                        column: x => x.CarModelId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cars_CategoryId",
                table: "Cars",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_MakesCompaneyId",
                table: "Cars",
                column: "MakesCompaneyId");

            migrationBuilder.CreateIndex(
                name: "IX_carsColors_CarModelId",
                table: "carsColors",
                column: "CarModelId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryMakesCompaney_CategoryId",
                table: "CategoryMakesCompaney",
                column: "CategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "carsColors");

            migrationBuilder.DropTable(
                name: "CategoryMakesCompaney");

            migrationBuilder.DropTable(
                name: "Cars");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "MakesCompaneys");
        }
    }
}
