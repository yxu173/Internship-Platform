using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class EditRoadmapRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoadmapItems_RoadmapSections_RoadmapSectionId",
                table: "RoadmapItems");

            migrationBuilder.DropForeignKey(
                name: "FK_RoadmapSections_Roadmaps_RoadmapId",
                table: "RoadmapSections");

            migrationBuilder.DropIndex(
                name: "IX_RoadmapItems_RoadmapSectionId",
                table: "RoadmapItems");

            migrationBuilder.DropColumn(
                name: "RoadmapSectionId",
                table: "RoadmapItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "RoadmapId",
                table: "RoadmapSections",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SectionId",
                table: "RoadmapItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_RoadmapItems_SectionId",
                table: "RoadmapItems",
                column: "SectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoadmapItems_RoadmapSections_SectionId",
                table: "RoadmapItems",
                column: "SectionId",
                principalTable: "RoadmapSections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoadmapSections_Roadmaps_RoadmapId",
                table: "RoadmapSections",
                column: "RoadmapId",
                principalTable: "Roadmaps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoadmapItems_RoadmapSections_SectionId",
                table: "RoadmapItems");

            migrationBuilder.DropForeignKey(
                name: "FK_RoadmapSections_Roadmaps_RoadmapId",
                table: "RoadmapSections");

            migrationBuilder.DropIndex(
                name: "IX_RoadmapItems_SectionId",
                table: "RoadmapItems");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "RoadmapItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "RoadmapId",
                table: "RoadmapSections",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "RoadmapSectionId",
                table: "RoadmapItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoadmapItems_RoadmapSectionId",
                table: "RoadmapItems",
                column: "RoadmapSectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoadmapItems_RoadmapSections_RoadmapSectionId",
                table: "RoadmapItems",
                column: "RoadmapSectionId",
                principalTable: "RoadmapSections",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoadmapSections_Roadmaps_RoadmapId",
                table: "RoadmapSections",
                column: "RoadmapId",
                principalTable: "Roadmaps",
                principalColumn: "Id");
        }
    }
}
