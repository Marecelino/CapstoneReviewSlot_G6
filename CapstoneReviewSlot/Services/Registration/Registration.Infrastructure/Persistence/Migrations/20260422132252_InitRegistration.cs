using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Registration.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentSlotPreference",
                columns: table => new
                {
                    PreferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CapstoneGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewSlotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PreferenceOrder = table.Column<int>(type: "int", nullable: false),
                    StudentMssv = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentSlotPreference", x => x.PreferenceId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentSlotPreference_CapstoneGroupId_ReviewSlotId_StudentMssv",
                table: "StudentSlotPreference",
                columns: new[] { "CapstoneGroupId", "ReviewSlotId", "StudentMssv" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentSlotPreference");
        }
    }
}
