using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramBot.Migrations
{
    /// <inheritdoc />
    public partial class addComletedTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserTasks");

            migrationBuilder.AddColumn<bool>(
                name: "Completed",
                table: "UserTasks",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Completed",
                table: "UserTasks");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "UserTasks",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
