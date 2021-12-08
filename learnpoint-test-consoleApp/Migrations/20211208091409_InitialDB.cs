using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace learnpoint_test_consoleApp.Migrations
{
    public partial class InitialDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    SourceId_IntId = table.Column<int>(type: "INTEGER", nullable: true),
                    SourceId_GuidId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SourceId_IType = table.Column<int>(type: "INTEGER", nullable: true),
                    SourceId_SType = table.Column<int>(type: "INTEGER", nullable: true),
                    TargetId_IntId = table.Column<int>(type: "INTEGER", nullable: true),
                    TargetId_GuidId = table.Column<Guid>(type: "TEXT", nullable: true),
                    TargetId_IType = table.Column<int>(type: "INTEGER", nullable: true),
                    TargetId_SType = table.Column<int>(type: "INTEGER", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Resources");
        }
    }
}
