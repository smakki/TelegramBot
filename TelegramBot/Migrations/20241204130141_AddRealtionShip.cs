using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramBot.Migrations
{
    /// <inheritdoc />
    public partial class AddRealtionShip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "UserTasks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_UserTasks_UserId",
                table: "UserTasks",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTasks_Users_UserId",
                table: "UserTasks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTasks_Users_UserId",
                table: "UserTasks");

            migrationBuilder.DropIndex(
                name: "IX_UserTasks_UserId",
                table: "UserTasks");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserTasks");
        }
    }
}
