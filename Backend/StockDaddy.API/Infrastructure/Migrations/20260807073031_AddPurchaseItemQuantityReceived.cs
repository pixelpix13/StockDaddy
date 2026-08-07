using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockDaddy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseItemQuantityReceived : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuantityReceived",
                table: "PurchaseItems",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "FullyReceived",
                table: "PurchaseOrders",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullyReceived",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "QuantityReceived",
                table: "PurchaseItems");
        }
    }
}
