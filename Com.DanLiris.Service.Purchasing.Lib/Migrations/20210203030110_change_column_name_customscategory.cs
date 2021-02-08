using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.DanLiris.Service.Purchasing.Lib.Migrations
{
    public partial class change_column_name_customscategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CustomCategory",
                table: "GarmentUnitReceiptNoteItems",
                newName: "CustomsCategory");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CustomsCategory",
                table: "GarmentUnitReceiptNoteItems",
                newName: "CustomCategory");
        }
    }
}
