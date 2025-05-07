using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkinTelIigent.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ignoreId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "DoctorPatient");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ClinicDoctor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "DoctorPatient",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ClinicDoctor",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
