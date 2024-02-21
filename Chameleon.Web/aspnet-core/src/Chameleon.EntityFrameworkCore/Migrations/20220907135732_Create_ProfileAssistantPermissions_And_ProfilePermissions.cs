using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Chameleon.Migrations
{
    public partial class Create_ProfileAssistantPermissions_And_ProfilePermissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProfilePermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PermissionName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfilePermissions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProfileAssistantPermissions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProfileAssistantId = table.Column<long>(type: "bigint", nullable: false),
                    ProfilePermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileAssistantPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileAssistantPermissions_ProfileAssistant_ProfileAssistan~",
                        column: x => x.ProfileAssistantId,
                        principalTable: "ProfileAssistant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileAssistantPermissions_ProfilePermissions_ProfilePermis~",
                        column: x => x.ProfilePermissionId,
                        principalTable: "ProfilePermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileAssistantPermissions_ProfileAssistantId",
                table: "ProfileAssistantPermissions",
                column: "ProfileAssistantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileAssistantPermissions_ProfilePermissionId",
                table: "ProfileAssistantPermissions",
                column: "ProfilePermissionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfileAssistantPermissions");

            migrationBuilder.DropTable(
                name: "ProfilePermissions");
        }
    }
}
