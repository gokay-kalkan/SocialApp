using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class followrequestnotificationtableaddfollowrequestidcolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FollowRequestId",
                table: "FollowRequestNotifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FollowRequestNotifications_FollowRequestId",
                table: "FollowRequestNotifications",
                column: "FollowRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_FollowRequestNotifications_FollowRequests_FollowRequestId",
                table: "FollowRequestNotifications",
                column: "FollowRequestId",
                principalTable: "FollowRequests",
                principalColumn: "FollowRequestId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FollowRequestNotifications_FollowRequests_FollowRequestId",
                table: "FollowRequestNotifications");

            migrationBuilder.DropIndex(
                name: "IX_FollowRequestNotifications_FollowRequestId",
                table: "FollowRequestNotifications");

            migrationBuilder.DropColumn(
                name: "FollowRequestId",
                table: "FollowRequestNotifications");
        }
    }
}
