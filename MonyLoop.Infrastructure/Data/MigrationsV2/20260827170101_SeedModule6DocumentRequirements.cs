using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonyLoop.Infrastructure.Data.MigrationsV2
{
    /// <inheritdoc />
    public partial class SeedModule6DocumentRequirements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "DocumentRequirements",
                columns: new[] { "DocumentRequirementId", "Description", "DisplayOrder", "DocumentName", "IsActive", "IsRequired" },
                values: new object[] { new Guid("8f6a0f13-55f6-4d7e-b560-5c0d0c428a01"), "Clear copy of the member's National ID.", 1, "National ID Copy", true, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DocumentRequirements",
                keyColumn: "DocumentRequirementId",
                keyValue: new Guid("8f6a0f13-55f6-4d7e-b560-5c0d0c428a01"));
        }
    }
}
