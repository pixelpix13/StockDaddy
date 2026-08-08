using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StockDaddy.API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWholesaleCompanies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DO $$ BEGIN
                  IF NOT EXISTS (
                    SELECT 1 FROM information_schema.columns
                    WHERE table_name = 'Sales' AND column_name = 'CompanyId'
                  ) THEN
                    ALTER TABLE "Sales" ADD "CompanyId" integer;
                  END IF;
                END $$;
                """);

            migrationBuilder.Sql("""
                DO $$ BEGIN
                  IF NOT EXISTS (
                    SELECT 1 FROM information_schema.columns
                    WHERE table_name = 'CreditLedgers' AND column_name = 'CompanyId'
                  ) THEN
                    ALTER TABLE "CreditLedgers" ADD "CompanyId" integer;
                  END IF;
                END $$;
                """);

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ContactName = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Gstin = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Companies_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql("""
                CREATE INDEX IF NOT EXISTS "IX_Sales_CompanyId" ON "Sales" ("CompanyId");
                CREATE INDEX IF NOT EXISTS "IX_CreditLedgers_CompanyId" ON "CreditLedgers" ("CompanyId");
                CREATE INDEX IF NOT EXISTS "IX_Companies_TenantId" ON "Companies" ("TenantId");
                """);

            migrationBuilder.Sql("""
                DO $$ BEGIN
                  IF NOT EXISTS (
                    SELECT 1 FROM pg_constraint WHERE conname = 'FK_CreditLedgers_Companies_CompanyId'
                  ) THEN
                    ALTER TABLE "CreditLedgers"
                      ADD CONSTRAINT "FK_CreditLedgers_Companies_CompanyId"
                      FOREIGN KEY ("CompanyId") REFERENCES "Companies" ("Id");
                  END IF;
                END $$;
                """);

            migrationBuilder.Sql("""
                DO $$ BEGIN
                  IF NOT EXISTS (
                    SELECT 1 FROM pg_constraint WHERE conname = 'FK_Sales_Companies_CompanyId'
                  ) THEN
                    ALTER TABLE "Sales"
                      ADD CONSTRAINT "FK_Sales_Companies_CompanyId"
                      FOREIGN KEY ("CompanyId") REFERENCES "Companies" ("Id");
                  END IF;
                END $$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditLedgers_Companies_CompanyId",
                table: "CreditLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Companies_CompanyId",
                table: "Sales");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Sales_CompanyId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_CreditLedgers_CompanyId",
                table: "CreditLedgers");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "CreditLedgers");
        }
    }
}
