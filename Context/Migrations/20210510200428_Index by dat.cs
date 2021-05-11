using Microsoft.EntityFrameworkCore.Migrations;

namespace Context.Migrations
{
    public partial class Indexbydat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Notifies_Date",
                table: "Notifies",
                column: "Date");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Notifies_Date",
                table: "Notifies");
        }
    }
}
