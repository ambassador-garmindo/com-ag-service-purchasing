using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.DanLiris.Service.Purchasing.Lib.Migrations
{
    public partial class garment_stock_opnames_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GarmentStockOpnames",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    CreatedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    Date = table.Column<DateTimeOffset>(nullable: false),
                    DeletedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    StorageCode = table.Column<string>(maxLength: 25, nullable: true),
                    StorageId = table.Column<int>(nullable: false),
                    StorageName = table.Column<string>(maxLength: 255, nullable: true),
                    UnitCode = table.Column<string>(maxLength: 25, nullable: true),
                    UnitId = table.Column<int>(nullable: false),
                    UnitName = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentStockOpnames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GarmentStockOpnameItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    BeforeQuantity = table.Column<decimal>(nullable: false),
                    CreatedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    DOCurrencyRate = table.Column<double>(nullable: false),
                    DOItemId = table.Column<long>(nullable: false),
                    DOItemNo = table.Column<string>(maxLength: 255, nullable: true),
                    DeletedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(nullable: false),
                    DesignColor = table.Column<string>(maxLength: 255, nullable: true),
                    DetailReferenceId = table.Column<long>(nullable: false),
                    EPOItemId = table.Column<long>(nullable: false),
                    GarmentStockOpnameId = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    POId = table.Column<long>(nullable: false),
                    POItemId = table.Column<long>(nullable: false),
                    POSerialNumber = table.Column<string>(maxLength: 100, nullable: true),
                    PRItemId = table.Column<long>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    ProductCode = table.Column<string>(maxLength: 255, nullable: true),
                    ProductId = table.Column<long>(nullable: false),
                    ProductName = table.Column<string>(maxLength: 1000, nullable: true),
                    Quantity = table.Column<decimal>(nullable: false),
                    RO = table.Column<string>(maxLength: 255, nullable: true),
                    SmallQuantity = table.Column<decimal>(nullable: false),
                    SmallUomId = table.Column<long>(nullable: false),
                    SmallUomUnit = table.Column<string>(maxLength: 100, nullable: true),
                    StorageCode = table.Column<string>(maxLength: 255, nullable: true),
                    StorageId = table.Column<long>(nullable: false),
                    StorageName = table.Column<string>(maxLength: 100, nullable: true),
                    UId = table.Column<string>(maxLength: 100, nullable: true),
                    URNItemId = table.Column<long>(nullable: false),
                    UnitCode = table.Column<string>(maxLength: 255, nullable: true),
                    UnitId = table.Column<long>(nullable: false),
                    UnitName = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentStockOpnameItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarmentStockOpnameItems_GarmentStockOpnames_GarmentStockOpnameId",
                        column: x => x.GarmentStockOpnameId,
                        principalTable: "GarmentStockOpnames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GarmentStockOpnameItems_GarmentStockOpnameId",
                table: "GarmentStockOpnameItems",
                column: "GarmentStockOpnameId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GarmentStockOpnameItems");

            migrationBuilder.DropTable(
                name: "GarmentStockOpnames");
        }
    }
}
