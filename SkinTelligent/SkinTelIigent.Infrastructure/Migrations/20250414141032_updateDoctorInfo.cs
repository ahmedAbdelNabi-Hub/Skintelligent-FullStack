using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkinTelIigent.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateDoctorInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AboutMe",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsProfileCompleted",
                table: "Doctors",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AboutMe",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "IsProfileCompleted",
                table: "Doctors");
        }
    }
}
