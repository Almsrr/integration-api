using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IntegrationAPI.Migrations
{
    public partial class AddTransactions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaCreacion = table.Column<DateTime>(nullable: false),
                    MontoTransaccion = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IDCuentaEmisor = table.Column<int>(nullable: false),
                    IDCuentaReceptor = table.Column<int>(nullable: false),
                    TipoTransaccion = table.Column<string>(maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
