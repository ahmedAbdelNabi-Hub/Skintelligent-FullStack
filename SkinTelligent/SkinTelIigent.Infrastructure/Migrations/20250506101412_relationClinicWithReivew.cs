using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkinTelIigent.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class relationClinicWithReivew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Reviews",
                type: "DateTime",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "DateTime");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Reviews",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "ClinicId",
                table: "Reviews",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClinicId1",
                table: "Reviews",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ClinicId",
                table: "Reviews",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ClinicId1",
                table: "Reviews",
                column: "ClinicId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Clinic_ClinicId",
                table: "Reviews",
                column: "ClinicId",
                principalTable: "Clinic",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Clinic_ClinicId1",
                table: "Reviews",
                column: "ClinicId1",
                principalTable: "Clinic",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Clinic_ClinicId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Clinic_ClinicId1",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_ClinicId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_ClinicId1",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ClinicId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ClinicId1",
                table: "Reviews");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Reviews",
                type: "DateTime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DateTime",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300);
        }
    }
}
