using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockDaddy.API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserStores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserStores",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    StoreId = table.Column<int>(type: "integer", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStores", x => new { x.UserId, x.StoreId });
                    table.ForeignKey(
                        name: "FK_UserStores_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserStores_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserStores_StoreId",
                table: "UserStores",
                column: "StoreId");

            migrationBuilder.Sql("""
                INSERT INTO "UserStores" ("UserId", "StoreId", "IsDefault")
                SELECT u."Id", u."StoreId", true
                FROM "Users" u
                WHERE u."StoreId" IS NOT NULL
                  AND NOT u."IsDeleted"
                  AND EXISTS (
                    SELECT 1 FROM "Stores" s
                    WHERE s."Id" = u."StoreId" AND NOT s."IsDeleted"
                  );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserStores");
        }
    }
}
