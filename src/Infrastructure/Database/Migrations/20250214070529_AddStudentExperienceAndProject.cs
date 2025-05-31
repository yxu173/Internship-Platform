using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentExperienceAndProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EnrollmentYear",
                table: "StudentProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ResumeUrl",
                table: "StudentProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Governorate",
                table: "CompanyProfiles",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "Address_City",
                table: "CompanyProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Street",
                table: "CompanyProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "CompanyProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyProfileId",
                table: "Applications",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StudentExperiences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    JobTitle = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CompanyName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentExperiences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentExperiences_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentProjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ProjectUrl = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentProjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentProjects_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applications_CompanyProfileId",
                table: "Applications",
                column: "CompanyProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentExperiences_StudentProfileId",
                table: "StudentExperiences",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentProjects_StudentProfileId",
                table: "StudentProjects",
                column: "StudentProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_CompanyProfiles_CompanyProfileId",
                table: "Applications",
                column: "CompanyProfileId",
                principalTable: "CompanyProfiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_CompanyProfiles_CompanyProfileId",
                table: "Applications");

            migrationBuilder.DropTable(
                name: "StudentExperiences");

            migrationBuilder.DropTable(
                name: "StudentProjects");

            migrationBuilder.DropIndex(
                name: "IX_Applications_CompanyProfileId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "EnrollmentYear",
                table: "StudentProfiles");

            migrationBuilder.DropColumn(
                name: "ResumeUrl",
                table: "StudentProfiles");

            migrationBuilder.DropColumn(
                name: "Address_City",
                table: "CompanyProfiles");

            migrationBuilder.DropColumn(
                name: "Address_Street",
                table: "CompanyProfiles");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "CompanyProfiles");

            migrationBuilder.DropColumn(
                name: "CompanyProfileId",
                table: "Applications");

            migrationBuilder.AlterColumn<string>(
                name: "Governorate",
                table: "CompanyProfiles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
