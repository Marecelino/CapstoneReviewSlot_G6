using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Session.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCapstoneGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CapstoneGroup",
                columns: table => new
                {
                    CapstoneGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProjectCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProjectNameEn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ProjectNameVn = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    MentorLecturerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupervisorJson = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CapstoneGroup", x => x.CapstoneGroupId);
                    table.ForeignKey(
                        name: "FK_CapstoneGroup_ReviewCampaign_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "ReviewCampaign",
                        principalColumn: "CampaignId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CapstoneGroupMember",
                columns: table => new
                {
                    MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CapstoneGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentMssv = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StudentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CapstoneGroupMember", x => x.MemberId);
                    table.ForeignKey(
                        name: "FK_CapstoneGroupMember_CapstoneGroup_CapstoneGroupId",
                        column: x => x.CapstoneGroupId,
                        principalTable: "CapstoneGroup",
                        principalColumn: "CapstoneGroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CapstoneGroup_CampaignId",
                table: "CapstoneGroup",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CapstoneGroupMember_CapstoneGroupId_StudentMssv",
                table: "CapstoneGroupMember",
                columns: new[] { "CapstoneGroupId", "StudentMssv" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CapstoneGroupMember_StudentMssv",
                table: "CapstoneGroupMember",
                column: "StudentMssv");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CapstoneGroupMember");

            migrationBuilder.DropTable(
                name: "CapstoneGroup");
        }
    }
}
