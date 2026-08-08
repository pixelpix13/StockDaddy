using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockDaddy.API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStoreScopeToParties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "Suppliers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "Customers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "CreditLedgers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "Companies",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE "Customers" c
                SET "StoreId" = sub."StoreId"
                FROM (
                    SELECT c2."Id" AS "CustomerId",
                           COALESCE(
                               (SELECT MIN(s."Id") FROM "Stores" s WHERE s."TenantId" = c2."TenantId" AND NOT s."IsDeleted"),
                               1
                           ) AS "StoreId"
                    FROM "Customers" c2
                ) sub
                WHERE c."Id" = sub."CustomerId" AND c."StoreId" IS NULL;

                UPDATE "Suppliers" s
                SET "StoreId" = sub."StoreId"
                FROM (
                    SELECT s2."Id" AS "SupplierId",
                           COALESCE(
                               (SELECT MIN(st."Id") FROM "Stores" st WHERE st."TenantId" = s2."TenantId" AND NOT st."IsDeleted"),
                               1
                           ) AS "StoreId"
                    FROM "Suppliers" s2
                ) sub
                WHERE s."Id" = sub."SupplierId" AND s."StoreId" IS NULL;

                UPDATE "Companies" c
                SET "StoreId" = sub."StoreId"
                FROM (
                    SELECT c2."Id" AS "CompanyId",
                           COALESCE(
                               (SELECT MIN(s."Id") FROM "Stores" s WHERE s."TenantId" = c2."TenantId" AND NOT s."IsDeleted"),
                               1
                           ) AS "StoreId"
                    FROM "Companies" c2
                ) sub
                WHERE c."Id" = sub."CompanyId" AND c."StoreId" IS NULL;

                UPDATE "CreditLedgers" cl
                SET "StoreId" = COALESCE(
                    (SELECT sa."StoreId" FROM "Sales" sa WHERE sa."Id" = cl."SaleId"),
                    (SELECT po."StoreId" FROM "PurchaseOrders" po WHERE po."Id" = cl."PurchaseOrderId"),
                    (SELECT MIN(s."Id") FROM "Stores" s WHERE s."TenantId" = cl."TenantId" AND NOT s."IsDeleted"),
                    1
                )
                WHERE cl."StoreId" IS NULL;
                """);

            migrationBuilder.AlterColumn<int>(
                name: "StoreId",
                table: "Suppliers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StoreId",
                table: "Customers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StoreId",
                table: "CreditLedgers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StoreId",
                table: "Companies",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_StoreId",
                table: "Suppliers",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_StoreId",
                table: "Customers",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditLedgers_StoreId",
                table: "CreditLedgers",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_StoreId",
                table: "Companies",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Stores_StoreId",
                table: "Companies",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditLedgers_Stores_StoreId",
                table: "CreditLedgers",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Stores_StoreId",
                table: "Customers",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Suppliers_Stores_StoreId",
                table: "Suppliers",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Stores_StoreId",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditLedgers_Stores_StoreId",
                table: "CreditLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Stores_StoreId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Suppliers_Stores_StoreId",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_Suppliers_StoreId",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_StoreId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_CreditLedgers_StoreId",
                table: "CreditLedgers");

            migrationBuilder.DropIndex(
                name: "IX_Companies_StoreId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "CreditLedgers");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Companies");
        }
    }
}
