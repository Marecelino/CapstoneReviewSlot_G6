using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Availability.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvailabilityId",
                table: "LecturerAvailability",
                newName: "Id");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "LecturerAvailability",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "LecturerAvailability",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<DateTime>(
                name: "RegisteredAt",
                table: "LecturerAvailability",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "LecturerAvailability",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_LecturerAvailability_LecturerId",
                table: "LecturerAvailability",
                column: "LecturerId");

            migrationBuilder.CreateIndex(
                name: "IX_LecturerAvailability_ReviewSlotId",
                table: "LecturerAvailability",
                column: "ReviewSlotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LecturerAvailability_LecturerId",
                table: "LecturerAvailability");

            migrationBuilder.DropIndex(
                name: "IX_LecturerAvailability_ReviewSlotId",
                table: "LecturerAvailability");

            migrationBuilder.DropColumn(
                name: "RegisteredAt",
                table: "LecturerAvailability");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "LecturerAvailability");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "LecturerAvailability",
                newName: "AvailabilityId");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "LecturerAvailability",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<Guid>(
                name: "AvailabilityId",
                table: "LecturerAvailability",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWID()");
        }
    }
}
