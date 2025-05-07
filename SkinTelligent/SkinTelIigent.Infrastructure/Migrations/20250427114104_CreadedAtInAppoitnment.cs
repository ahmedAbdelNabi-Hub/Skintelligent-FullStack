using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkinTelIigent.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreadedAtInAppoitnment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Appointments",
                type: "DateTime",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Appointments");
        }
    }
}
