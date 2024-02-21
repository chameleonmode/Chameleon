using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Chameleon.Migrations
{
    public partial class Update_OutReachLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Facebook",
                table: "OutReachLinks",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Linkedin",
                table: "OutReachLinks",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "OtherSocial",
                table: "OutReachLinks",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReminderDatetime",
                table: "OutReachLinks",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReminderNotes",
                table: "OutReachLinks",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Twitter",
                table: "OutReachLinks",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Facebook",
                table: "OutReachLinks");

            migrationBuilder.DropColumn(
                name: "Linkedin",
                table: "OutReachLinks");

            migrationBuilder.DropColumn(
                name: "OtherSocial",
                table: "OutReachLinks");

            migrationBuilder.DropColumn(
                name: "ReminderDatetime",
                table: "OutReachLinks");

            migrationBuilder.DropColumn(
                name: "ReminderNotes",
                table: "OutReachLinks");

            migrationBuilder.DropColumn(
                name: "Twitter",
                table: "OutReachLinks");
        }
    }
}
