using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace learnpoint_test_consoleApp.Migrations
{
    public partial class InitialDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Resource",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceId_IntId = table.Column<int>(type: "int", nullable: true),
                    SourceId_GuidId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceId_IType = table.Column<int>(type: "int", nullable: true),
                    SourceId_SType = table.Column<int>(type: "int", nullable: true),
                    TargetId_IntId = table.Column<int>(type: "int", nullable: true),
                    TargetId_GuidId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TargetId_IType = table.Column<int>(type: "int", nullable: true),
                    TargetId_SType = table.Column<int>(type: "int", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resource", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Resource");
        }
    }
}
