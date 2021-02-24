using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.DanLiris.Service.Purchasing.Lib.Migrations
{
    public partial class remove_facility_type_garment_do_unit_recipt_note_item : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FacilityType",
                table: "GarmentUnitReceiptNoteItems");

            migrationBuilder.DropColumn(
                name: "FacilityType",
                table: "GarmentDeliveryOrders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FacilityType",
                table: "GarmentUnitReceiptNoteItems",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FacilityType",
                table: "GarmentDeliveryOrders",
                maxLength: 50,
                nullable: true);
        }
    }
}
