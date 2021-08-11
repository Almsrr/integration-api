using Microsoft.EntityFrameworkCore.Migrations;

namespace IntegrationAPI.Migrations
{
    public partial class ModifyTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Transactions");

            migrationBuilder.AddColumn<string>(
                name: "Descripción",
                table: "Transactions",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descripción",
                table: "Transactions");

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Transactions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
