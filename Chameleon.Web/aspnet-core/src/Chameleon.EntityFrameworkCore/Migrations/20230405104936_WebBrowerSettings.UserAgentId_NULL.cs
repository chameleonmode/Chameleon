using Microsoft.EntityFrameworkCore.Migrations;

namespace Chameleon.Migrations
{
    public partial class WebBrowerSettingsUserAgentId_NULL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WebBrowserSettings_WebBrowserUserAgents_UserAgentId",
                table: "WebBrowserSettings");

            migrationBuilder.AlterColumn<int>(
                name: "UserAgentId",
                table: "WebBrowserSettings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_WebBrowserSettings_WebBrowserUserAgents_UserAgentId",
                table: "WebBrowserSettings",
                column: "UserAgentId",
                principalTable: "WebBrowserUserAgents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WebBrowserSettings_WebBrowserUserAgents_UserAgentId",
                table: "WebBrowserSettings");

            migrationBuilder.AlterColumn<int>(
                name: "UserAgentId",
                table: "WebBrowserSettings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WebBrowserSettings_WebBrowserUserAgents_UserAgentId",
                table: "WebBrowserSettings",
                column: "UserAgentId",
                principalTable: "WebBrowserUserAgents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
