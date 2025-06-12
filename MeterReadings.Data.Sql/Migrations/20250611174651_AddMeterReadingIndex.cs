using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeterReadings.Data.Sql.Migrations
{
    /// <inheritdoc />
    public partial class AddMeterReadingIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MeterReadings_AccountId",
                table: "MeterReadings");

            migrationBuilder.CreateIndex(
                name: "IX_MeterReadings_AccountId_ReadingDateTime_Value",
                table: "MeterReadings",
                columns: new[] { "AccountId", "ReadingDateTime", "Value" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MeterReadings_AccountId_ReadingDateTime_Value",
                table: "MeterReadings");

            migrationBuilder.CreateIndex(
                name: "IX_MeterReadings_AccountId",
                table: "MeterReadings",
                column: "AccountId");
        }
    }
}
