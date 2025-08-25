using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarAuction.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuctionVehicles_Vehicle_VehicleId",
                table: "AuctionVehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleImage_Vehicle_VehicleId",
                table: "VehicleImage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleImage",
                table: "VehicleImage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vehicle",
                table: "Vehicle");

            migrationBuilder.RenameTable(
                name: "VehicleImage",
                newName: "VehicleImages");

            migrationBuilder.RenameTable(
                name: "Vehicle",
                newName: "Vehicles");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleImage_VehicleId",
                table: "VehicleImages",
                newName: "IX_VehicleImages_VehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_Vehicle_VIN",
                table: "Vehicles",
                newName: "IX_Vehicles_VIN");

            migrationBuilder.AddColumn<Guid>(
                name: "VehicleId1",
                table: "AuctionVehicles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VIN",
                table: "Vehicles",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(17)",
                oldMaxLength: 17);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Vehicles",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Transmission",
                table: "Vehicles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ModelType",
                table: "Vehicles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Make",
                table: "Vehicles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Vehicles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Grade",
                table: "Vehicles",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(3,1)");

            migrationBuilder.AlterColumn<string>(
                name: "FuelType",
                table: "Vehicles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Vehicles",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "Vehicles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BodyStyle",
                table: "Vehicles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleImages",
                table: "VehicleImages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vehicles",
                table: "Vehicles",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionVehicles_VehicleId1",
                table: "AuctionVehicles",
                column: "VehicleId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionVehicles_Vehicles_VehicleId",
                table: "AuctionVehicles",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionVehicles_Vehicles_VehicleId1",
                table: "AuctionVehicles",
                column: "VehicleId1",
                principalTable: "Vehicles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleImages_Vehicles_VehicleId",
                table: "VehicleImages",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuctionVehicles_Vehicles_VehicleId",
                table: "AuctionVehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_AuctionVehicles_Vehicles_VehicleId1",
                table: "AuctionVehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleImages_Vehicles_VehicleId",
                table: "VehicleImages");

            migrationBuilder.DropIndex(
                name: "IX_AuctionVehicles_VehicleId1",
                table: "AuctionVehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vehicles",
                table: "Vehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleImages",
                table: "VehicleImages");

            migrationBuilder.DropColumn(
                name: "VehicleId1",
                table: "AuctionVehicles");

            migrationBuilder.RenameTable(
                name: "Vehicles",
                newName: "Vehicle");

            migrationBuilder.RenameTable(
                name: "VehicleImages",
                newName: "VehicleImage");

            migrationBuilder.RenameIndex(
                name: "IX_Vehicles_VIN",
                table: "Vehicle",
                newName: "IX_Vehicle_VIN");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleImages_VehicleId",
                table: "VehicleImage",
                newName: "IX_VehicleImage_VehicleId");

            migrationBuilder.AlterColumn<string>(
                name: "VIN",
                table: "Vehicle",
                type: "nvarchar(17)",
                maxLength: 17,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Vehicle",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "Transmission",
                table: "Vehicle",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ModelType",
                table: "Vehicle",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Make",
                table: "Vehicle",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Vehicle",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Grade",
                table: "Vehicle",
                type: "decimal(3,1)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "FuelType",
                table: "Vehicle",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Vehicle",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "Vehicle",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "BodyStyle",
                table: "Vehicle",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vehicle",
                table: "Vehicle",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleImage",
                table: "VehicleImage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionVehicles_Vehicle_VehicleId",
                table: "AuctionVehicles",
                column: "VehicleId",
                principalTable: "Vehicle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleImage_Vehicle_VehicleId",
                table: "VehicleImage",
                column: "VehicleId",
                principalTable: "Vehicle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
