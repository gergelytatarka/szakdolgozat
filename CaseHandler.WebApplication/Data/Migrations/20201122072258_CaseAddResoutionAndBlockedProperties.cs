using Microsoft.EntityFrameworkCore.Migrations;

namespace CaseHandler.WebApplication.Data.Migrations
{
    public partial class CaseAddResoutionAndBlockedProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "Notifications",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<bool>(
                name: "Blocked",
                table: "Cases",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Resolution",
                table: "Cases",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Blocked",
                table: "Cases");

            migrationBuilder.DropColumn(
                name: "Resolution",
                table: "Cases");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "Notifications",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
