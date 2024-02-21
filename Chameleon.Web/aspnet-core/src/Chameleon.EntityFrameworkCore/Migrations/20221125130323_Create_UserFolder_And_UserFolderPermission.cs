using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Chameleon.Migrations
{
    public partial class Create_UserFolder_And_UserFolderPermission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsersFolders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    FolderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersFolders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersFolders_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersFolders_Folders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "Folders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserFoldersPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserFolderId = table.Column<int>(type: "int", nullable: false),
                    ProfilePermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFoldersPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFoldersPermissions_ProfilePermissions_ProfilePermissionId",
                        column: x => x.ProfilePermissionId,
                        principalTable: "ProfilePermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFoldersPermissions_UsersFolders_UserFolderId",
                        column: x => x.UserFolderId,
                        principalTable: "UsersFolders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_UserFoldersPermissions_ProfilePermissionId",
                table: "UserFoldersPermissions",
                column: "ProfilePermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFoldersPermissions_UserFolderId",
                table: "UserFoldersPermissions",
                column: "UserFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersFolders_FolderId",
                table: "UsersFolders",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersFolders_UserId",
                table: "UsersFolders",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFoldersPermissions");

            migrationBuilder.DropTable(
                name: "UsersFolders");
        }
    }
}
