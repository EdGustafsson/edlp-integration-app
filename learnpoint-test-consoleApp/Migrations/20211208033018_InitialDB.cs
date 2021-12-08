using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace learnpoint_test_consoleApp.Migrations
{
    public partial class InitialDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExternalId",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    IntId = table.Column<int>(type: "INTEGER", nullable: false),
                    GuidId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IType = table.Column<int>(type: "INTEGER", nullable: false),
                    SType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalId", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    SourceIdId = table.Column<Guid>(type: "TEXT", nullable: true),
                    TargetIdId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resources_ExternalId_SourceIdId",
                        column: x => x.SourceIdId,
                        principalTable: "ExternalId",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Resources_ExternalId_TargetIdId",
                        column: x => x.TargetIdId,
                        principalTable: "ExternalId",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Resources_SourceIdId",
                table: "Resources",
                column: "SourceIdId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_TargetIdId",
                table: "Resources",
                column: "TargetIdId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Resources");

            migrationBuilder.DropTable(
                name: "ExternalId");
        }
    }
}
