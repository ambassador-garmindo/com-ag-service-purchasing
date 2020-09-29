using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.DanLiris.Service.Purchasing.Lib.Migrations
{
    public partial class update_db_02042020to10092020 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ImportDuty",
                table: "UnitPaymentOrders",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "IsCreatedVB",
                table: "UnitPaymentOrders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPosted",
                table: "UnitPaymentOrders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PibDate",
                table: "UnitPaymentOrders",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<double>(
                name: "TotalIncomeTaxAmount",
                table: "UnitPaymentOrders",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalVatAmount",
                table: "UnitPaymentOrders",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "UId",
                table: "GarmentUnitReceiptNoteItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemStatus",
                table: "GarmentUnitExpenditureNoteItems",
                maxLength: 25,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UId",
                table: "GarmentUnitExpenditureNoteItems",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProductRemark",
                table: "GarmentUnitDeliveryOrderItems",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DesignColor",
                table: "GarmentUnitDeliveryOrderItems",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DOItemsId",
                table: "GarmentUnitDeliveryOrderItems",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UId",
                table: "GarmentUnitDeliveryOrderItems",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCreatedVB",
                table: "GarmentInternNotes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DivisionCode",
                table: "GarmentInternalPurchaseOrders",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DivisionId",
                table: "GarmentInternalPurchaseOrders",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DivisionName",
                table: "GarmentInternalPurchaseOrders",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UId",
                table: "GarmentDeliveryOrderItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Uid",
                table: "GarmentDeliveryOrderDetails",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCreateOnVBRequest",
                table: "ExternalPurchaseOrders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "POCashType",
                table: "ExternalPurchaseOrders",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SupplierIsImport",
                table: "ExternalPurchaseOrders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "BalanceStocks",
                columns: table => new
                {
                    BalanceStockId = table.Column<string>(type: "varchar(30)", nullable: false),
                    ArticleNo = table.Column<string>(type: "varchar(50)", nullable: true),
                    ClosePrice = table.Column<decimal>(type: "Money", nullable: true),
                    CloseStock = table.Column<double>(nullable: true),
                    CreateDate = table.Column<DateTime>(type: "Datetime", nullable: true),
                    CreditPrice = table.Column<decimal>(type: "Money", nullable: true),
                    CreditStock = table.Column<double>(nullable: true),
                    DebitPrice = table.Column<decimal>(type: "Money", nullable: true),
                    DebitStock = table.Column<double>(nullable: true),
                    OpenPrice = table.Column<decimal>(type: "Money", nullable: true),
                    OpenStock = table.Column<double>(nullable: true),
                    POID = table.Column<string>(type: "varchar(100)", nullable: true),
                    POItemId = table.Column<int>(nullable: true),
                    PeriodeMonth = table.Column<string>(type: "varchar(50)", nullable: true),
                    PeriodeYear = table.Column<string>(type: "varchar(10)", nullable: true),
                    RO = table.Column<string>(type: "varchar(50)", nullable: true),
                    SmallestUnitQty = table.Column<string>(type: "varchar(50)", nullable: true),
                    StockId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BalanceStocks", x => x.BalanceStockId);
                });

            migrationBuilder.CreateTable(
                name: "GarmentDOItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    CreatedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    DOCurrencyRate = table.Column<double>(nullable: false),
                    DOItemNo = table.Column<string>(maxLength: 255, nullable: true),
                    DeletedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(nullable: false),
                    DesignColor = table.Column<string>(maxLength: 255, nullable: true),
                    DetailReferenceId = table.Column<long>(nullable: false),
                    EPOItemId = table.Column<long>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    POId = table.Column<long>(nullable: false),
                    POItemId = table.Column<long>(nullable: false),
                    POSerialNumber = table.Column<string>(maxLength: 100, nullable: true),
                    PRItemId = table.Column<long>(nullable: false),
                    ProductCode = table.Column<string>(maxLength: 255, nullable: true),
                    ProductId = table.Column<long>(nullable: false),
                    ProductName = table.Column<string>(maxLength: 1000, nullable: true),
                    RO = table.Column<string>(maxLength: 255, nullable: true),
                    RemainingQuantity = table.Column<decimal>(nullable: false),
                    SmallQuantity = table.Column<decimal>(nullable: false),
                    SmallUomId = table.Column<long>(nullable: false),
                    SmallUomUnit = table.Column<string>(maxLength: 100, nullable: true),
                    StorageCode = table.Column<string>(maxLength: 255, nullable: true),
                    StorageId = table.Column<long>(nullable: false),
                    StorageName = table.Column<string>(maxLength: 100, nullable: true),
                    UId = table.Column<string>(nullable: true),
                    URNItemId = table.Column<long>(nullable: false),
                    UnitCode = table.Column<string>(maxLength: 255, nullable: true),
                    UnitId = table.Column<long>(nullable: false),
                    UnitName = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentDOItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GarmentSupplierBalanceDebts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    CodeRequirment = table.Column<string>(nullable: true),
                    CreatedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    DOCurrencyCode = table.Column<string>(nullable: true),
                    DOCurrencyId = table.Column<long>(nullable: true),
                    DOCurrencyRate = table.Column<double>(nullable: false),
                    DeletedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(nullable: false),
                    Import = table.Column<bool>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    SupplierCode = table.Column<string>(maxLength: 255, nullable: true),
                    SupplierId = table.Column<long>(maxLength: 255, nullable: false),
                    SupplierName = table.Column<string>(maxLength: 1000, nullable: true),
                    TotalAmountIDR = table.Column<double>(nullable: false),
                    TotalValas = table.Column<double>(nullable: false),
                    UId = table.Column<string>(maxLength: 255, nullable: true),
                    Year = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentSupplierBalanceDebts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GarmentSupplierBalanceDebtItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    ArrivalDate = table.Column<DateTimeOffset>(nullable: false),
                    BillNo = table.Column<string>(maxLength: 50, nullable: true),
                    CreatedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    DOId = table.Column<long>(nullable: false),
                    DONo = table.Column<string>(nullable: true),
                    DeletedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(nullable: false),
                    GarmentDebtId = table.Column<long>(nullable: false),
                    IDR = table.Column<double>(nullable: false),
                    InternNo = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    PaymentMethod = table.Column<string>(nullable: true),
                    PaymentType = table.Column<string>(nullable: true),
                    UId = table.Column<string>(maxLength: 255, nullable: true),
                    Valas = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentSupplierBalanceDebtItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarmentSupplierBalanceDebtItems_GarmentSupplierBalanceDebts_GarmentDebtId",
                        column: x => x.GarmentDebtId,
                        principalTable: "GarmentSupplierBalanceDebts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GarmentSupplierBalanceDebtItems_GarmentDebtId",
                table: "GarmentSupplierBalanceDebtItems",
                column: "GarmentDebtId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BalanceStocks");

            migrationBuilder.DropTable(
                name: "GarmentDOItems");

            migrationBuilder.DropTable(
                name: "GarmentSupplierBalanceDebtItems");

            migrationBuilder.DropTable(
                name: "GarmentSupplierBalanceDebts");

            migrationBuilder.DropColumn(
                name: "ImportDuty",
                table: "UnitPaymentOrders");

            migrationBuilder.DropColumn(
                name: "IsCreatedVB",
                table: "UnitPaymentOrders");

            migrationBuilder.DropColumn(
                name: "IsPosted",
                table: "UnitPaymentOrders");

            migrationBuilder.DropColumn(
                name: "PibDate",
                table: "UnitPaymentOrders");

            migrationBuilder.DropColumn(
                name: "TotalIncomeTaxAmount",
                table: "UnitPaymentOrders");

            migrationBuilder.DropColumn(
                name: "TotalVatAmount",
                table: "UnitPaymentOrders");

            migrationBuilder.DropColumn(
                name: "UId",
                table: "GarmentUnitReceiptNoteItems");

            migrationBuilder.DropColumn(
                name: "ItemStatus",
                table: "GarmentUnitExpenditureNoteItems");

            migrationBuilder.DropColumn(
                name: "UId",
                table: "GarmentUnitExpenditureNoteItems");

            migrationBuilder.DropColumn(
                name: "DOItemsId",
                table: "GarmentUnitDeliveryOrderItems");

            migrationBuilder.DropColumn(
                name: "UId",
                table: "GarmentUnitDeliveryOrderItems");

            migrationBuilder.DropColumn(
                name: "IsCreatedVB",
                table: "GarmentInternNotes");

            migrationBuilder.DropColumn(
                name: "DivisionCode",
                table: "GarmentInternalPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "GarmentInternalPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "DivisionName",
                table: "GarmentInternalPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "UId",
                table: "GarmentDeliveryOrderItems");

            migrationBuilder.DropColumn(
                name: "Uid",
                table: "GarmentDeliveryOrderDetails");

            migrationBuilder.DropColumn(
                name: "IsCreateOnVBRequest",
                table: "ExternalPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "POCashType",
                table: "ExternalPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "SupplierIsImport",
                table: "ExternalPurchaseOrders");

            migrationBuilder.AlterColumn<string>(
                name: "ProductRemark",
                table: "GarmentUnitDeliveryOrderItems",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DesignColor",
                table: "GarmentUnitDeliveryOrderItems",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 1000,
                oldNullable: true);
        }
    }
}
