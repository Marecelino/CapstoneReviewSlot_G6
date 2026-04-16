using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assignment.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReviewAssignment",
                columns: table => new
                {
                    ReviewAssignmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CapstoneGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewSlotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReviewOrder = table.Column<int>(type: "int", nullable: false),
                    AssignedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewAssignment", x => x.ReviewAssignmentId);
                });

            migrationBuilder.CreateTable(
                name: "ReviewAssignmentReviewer",
                columns: table => new
                {
                    ReviewerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LecturerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewAssignmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewAssignmentReviewer", x => x.ReviewerId);
                    table.ForeignKey(
                        name: "FK_ReviewAssignmentReviewer_ReviewAssignment_ReviewAssignmentId",
                        column: x => x.ReviewAssignmentId,
                        principalTable: "ReviewAssignment",
                        principalColumn: "ReviewAssignmentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewAssignment_CapstoneGroupId_ReviewSlotId",
                table: "ReviewAssignment",
                columns: new[] { "CapstoneGroupId", "ReviewSlotId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewAssignment_ReviewSlotId_ReviewOrder",
                table: "ReviewAssignment",
                columns: new[] { "ReviewSlotId", "ReviewOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewAssignmentReviewer_LecturerId_ReviewAssignmentId",
                table: "ReviewAssignmentReviewer",
                columns: new[] { "LecturerId", "ReviewAssignmentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewAssignmentReviewer_ReviewAssignmentId",
                table: "ReviewAssignmentReviewer",
                column: "ReviewAssignmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReviewAssignmentReviewer");

            migrationBuilder.DropTable(
                name: "ReviewAssignment");
        }
    }
}
