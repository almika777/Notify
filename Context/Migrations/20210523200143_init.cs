using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Context.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatUsers",
                columns: table => new
                {
                    ChatId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatUsers", x => x.ChatId);
                });

            migrationBuilder.CreateTable(
                name: "Notifies",
                columns: table => new
                {
                    NotifyId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserChatId = table.Column<long>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Frequency = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    FrequencyTime = table.Column<TimeSpan>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifies", x => x.NotifyId);
                    table.ForeignKey(
                        name: "FK_Notifies_ChatUsers_UserChatId",
                        column: x => x.UserChatId,
                        principalTable: "ChatUsers",
                        principalColumn: "ChatId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifies_Date",
                table: "Notifies",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Notifies_UserChatId",
                table: "Notifies",
                column: "UserChatId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifies");

            migrationBuilder.DropTable(
                name: "ChatUsers");
        }
    }
}
