using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RenameInqDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IquiryDate",
                table: "InquiryHeaders",
                newName: "InquiryDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InquiryDate",
                table: "InquiryHeaders",
                newName: "IquiryDate");
        }
    }
}
