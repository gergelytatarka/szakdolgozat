using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CaseHandler.WebApplication.Data.Migrations
{
    public partial class CaseAddDeadlineAndPriorityProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Deadline",
                table: "Cases",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Cases",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deadline",
                table: "Cases");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Cases");
        }
    }
}
