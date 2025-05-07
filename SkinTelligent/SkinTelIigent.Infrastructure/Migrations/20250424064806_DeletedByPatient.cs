using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkinTelIigent.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeletedByPatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Clinic",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeletedByPatient",
                table: "Appointments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PatientDeletedAt",
                table: "Appointments",
                type: "DateTime",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Clinic");

            migrationBuilder.DropColumn(
                name: "IsDeletedByPatient",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PatientDeletedAt",
                table: "Appointments");
        }
    }
}
