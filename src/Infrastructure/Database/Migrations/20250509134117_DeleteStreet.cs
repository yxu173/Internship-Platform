using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class DeleteStreet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address_Street",
                table: "CompanyProfiles");

            migrationBuilder.AddColumn<string>(
                name: "YearOfEstablishment",
                table: "CompanyProfiles",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YearOfEstablishment",
                table: "CompanyProfiles");

            migrationBuilder.AddColumn<string>(
                name: "Address_Street",
                table: "CompanyProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
