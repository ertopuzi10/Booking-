using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyPricingRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BaseGuestsIncluded",
                table: "Properties",
                type: "int",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.AddColumn<decimal>(
                name: "CleaningFee",
                table: "Properties",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ExtraGuestFeePerNight",
                table: "Properties",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ServiceFeePercent",
                table: "Properties",
                type: "decimal(5,4)",
                nullable: false,
                defaultValue: 0.10m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxPercent",
                table: "Properties",
                type: "decimal(5,4)",
                nullable: false,
                defaultValue: 0.085m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseGuestsIncluded",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "CleaningFee",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "ExtraGuestFeePerNight",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "ServiceFeePercent",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "TaxPercent",
                table: "Properties");
        }
    }
}
