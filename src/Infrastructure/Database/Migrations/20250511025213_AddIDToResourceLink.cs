using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddIDToResourceLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ResourceLink",
                table: "ResourceLink");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResourceLink",
                table: "ResourceLink",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceLink_RoadmapItemId",
                table: "ResourceLink",
                column: "RoadmapItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ResourceLink",
                table: "ResourceLink");

            migrationBuilder.DropIndex(
                name: "IX_ResourceLink_RoadmapItemId",
                table: "ResourceLink");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResourceLink",
                table: "ResourceLink",
                columns: new[] { "RoadmapItemId", "Id" });
        }
    }
}
