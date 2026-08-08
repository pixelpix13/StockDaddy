using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockDaddy.API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleIdToUserStores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                table: "UserStores",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE "UserStores" us
                SET "RoleId" = u."RoleId"
                FROM "Users" u
                WHERE us."UserId" = u."Id" AND us."RoleId" IS NULL;
                """);

            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                table: "UserStores",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserStores_RoleId",
                table: "UserStores",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserStores_Roles_RoleId",
                table: "UserStores",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserStores_Roles_RoleId",
                table: "UserStores");

            migrationBuilder.DropIndex(
                name: "IX_UserStores_RoleId",
                table: "UserStores");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "UserStores");
        }
    }
}
