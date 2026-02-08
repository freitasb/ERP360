using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP360.Estoque.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Estoque : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EstoqueItens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProdutoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuantidadeDisponivel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstoqueItens", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EstoqueItens_ProdutoId",
                table: "EstoqueItens",
                column: "ProdutoId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EstoqueItens");
        }
    }
}
