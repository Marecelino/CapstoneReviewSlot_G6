using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Session.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialSessionMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeOnly>(
                name: "StartTime",
                table: "ReviewSlot",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "ReviewDate",
                table: "ReviewSlot",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "EndTime",
                table: "ReviewSlot",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "ReviewCampaign",
                type: "int",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "MaxGroupsPerLecturer",
                table: "ReviewCampaign",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequiredReviewersPerGroup",
                table: "ReviewCampaign",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxGroupsPerLecturer",
                table: "ReviewCampaign");

            migrationBuilder.DropColumn(
                name: "RequiredReviewersPerGroup",
                table: "ReviewCampaign");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "ReviewSlot",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReviewDate",
                table: "ReviewSlot",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "ReviewSlot",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ReviewCampaign",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 50);
        }
    }
}
