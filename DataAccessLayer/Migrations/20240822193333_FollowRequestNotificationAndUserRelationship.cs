using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class FollowRequestNotificationAndUserRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "FollowRequestNotifications",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_FollowRequestNotifications_UserId",
                table: "FollowRequestNotifications",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FollowRequestNotifications_AspNetUsers_UserId",
                table: "FollowRequestNotifications",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FollowRequestNotifications_AspNetUsers_UserId",
                table: "FollowRequestNotifications");

            migrationBuilder.DropIndex(
                name: "IX_FollowRequestNotifications_UserId",
                table: "FollowRequestNotifications");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "FollowRequestNotifications");
        }
    }
}
