using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class EditInternshipEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Internships",
                newName: "WorkingModel");

            migrationBuilder.AddColumn<string>(
                name: "About",
                table: "Internships",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Internships",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "KeyResponsibilities",
                table: "Internships",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Requirements",
                table: "Internships",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Salary",
                table: "Internships",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "About",
                table: "Internships");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Internships");

            migrationBuilder.DropColumn(
                name: "KeyResponsibilities",
                table: "Internships");

            migrationBuilder.DropColumn(
                name: "Requirements",
                table: "Internships");

            migrationBuilder.DropColumn(
                name: "Salary",
                table: "Internships");

            migrationBuilder.RenameColumn(
                name: "WorkingModel",
                table: "Internships",
                newName: "Description");
        }
    }
}
