using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RenameRowsInTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Sqft",
                table: "OrderDetails",
                newName: "Count");

            migrationBuilder.RenameColumn(
                name: "PricePerSqFt",
                table: "OrderDetails",
                newName: "PricePerOne");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PricePerOne",
                table: "OrderDetails",
                newName: "PricePerSqFt");

            migrationBuilder.RenameColumn(
                name: "Count",
                table: "OrderDetails",
                newName: "Sqft");
        }
    }
}
