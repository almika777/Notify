using Microsoft.EntityFrameworkCore.Migrations;

namespace Context.Migrations
{
    public partial class removeprpty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NextStep",
                table: "Notifies");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Notifies",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Notifies",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NextStep",
                table: "Notifies",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
