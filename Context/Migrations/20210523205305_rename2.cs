using Microsoft.EntityFrameworkCore.Migrations;

namespace Context.Migrations
{
    public partial class rename2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifies_ChatUsers_UserChatId",
                table: "Notifies");

            migrationBuilder.RenameColumn(
                name: "UserChatId",
                table: "Notifies",
                newName: "ChatId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifies_UserChatId",
                table: "Notifies",
                newName: "IX_Notifies_ChatId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifies_ChatUsers_ChatId",
                table: "Notifies",
                column: "ChatId",
                principalTable: "ChatUsers",
                principalColumn: "ChatId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifies_ChatUsers_ChatId",
                table: "Notifies");

            migrationBuilder.RenameColumn(
                name: "ChatId",
                table: "Notifies",
                newName: "UserChatId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifies_ChatId",
                table: "Notifies",
                newName: "IX_Notifies_UserChatId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifies_ChatUsers_UserChatId",
                table: "Notifies",
                column: "UserChatId",
                principalTable: "ChatUsers",
                principalColumn: "ChatId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
