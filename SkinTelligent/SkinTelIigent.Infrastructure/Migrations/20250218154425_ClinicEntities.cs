using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkinTelIigent.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ClinicEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClinicDoctor_Clinic_ClinicsId",
                table: "ClinicDoctor");

            migrationBuilder.DropForeignKey(
                name: "FK_ClinicDoctor_Doctors_DoctorsId",
                table: "ClinicDoctor");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorPatient_Doctors_DoctorId",
                table: "DoctorPatient");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorPatient_Patients_PatientId",
                table: "DoctorPatient");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DoctorPatient",
                table: "DoctorPatient");

            migrationBuilder.DropIndex(
                name: "IX_DoctorPatient_PatientId",
                table: "DoctorPatient");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClinicDoctor",
                table: "ClinicDoctor");

            migrationBuilder.DropIndex(
                name: "IX_ClinicDoctor_DoctorsId",
                table: "ClinicDoctor");

            migrationBuilder.RenameColumn(
                name: "DoctorsId",
                table: "ClinicDoctor",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ClinicsId",
                table: "ClinicDoctor",
                newName: "DoctorId");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "DoctorPatient",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClinicId",
                table: "ClinicDoctor",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Clinic",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Clinic",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DoctorPatient",
                table: "DoctorPatient",
                columns: new[] { "PatientId", "DoctorId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClinicDoctor",
                table: "ClinicDoctor",
                columns: new[] { "ClinicId", "DoctorId" });

            migrationBuilder.CreateIndex(
                name: "IX_DoctorPatient_DoctorId",
                table: "DoctorPatient",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicDoctor_DoctorId",
                table: "ClinicDoctor",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Clinic_UserId",
                table: "Clinic",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Clinic_AspNetUsers_UserId",
                table: "Clinic",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClinicDoctor_Clinic_ClinicId",
                table: "ClinicDoctor",
                column: "ClinicId",
                principalTable: "Clinic",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClinicDoctor_Doctors_DoctorId",
                table: "ClinicDoctor",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorPatient_Doctors_DoctorId",
                table: "DoctorPatient",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorPatient_Patients_PatientId",
                table: "DoctorPatient",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clinic_AspNetUsers_UserId",
                table: "Clinic");

            migrationBuilder.DropForeignKey(
                name: "FK_ClinicDoctor_Clinic_ClinicId",
                table: "ClinicDoctor");

            migrationBuilder.DropForeignKey(
                name: "FK_ClinicDoctor_Doctors_DoctorId",
                table: "ClinicDoctor");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorPatient_Doctors_DoctorId",
                table: "DoctorPatient");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorPatient_Patients_PatientId",
                table: "DoctorPatient");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DoctorPatient",
                table: "DoctorPatient");

            migrationBuilder.DropIndex(
                name: "IX_DoctorPatient_DoctorId",
                table: "DoctorPatient");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClinicDoctor",
                table: "ClinicDoctor");

            migrationBuilder.DropIndex(
                name: "IX_ClinicDoctor_DoctorId",
                table: "ClinicDoctor");

            migrationBuilder.DropIndex(
                name: "IX_Clinic_UserId",
                table: "Clinic");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "DoctorPatient");

            migrationBuilder.DropColumn(
                name: "ClinicId",
                table: "ClinicDoctor");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Clinic");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Clinic");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ClinicDoctor",
                newName: "DoctorsId");

            migrationBuilder.RenameColumn(
                name: "DoctorId",
                table: "ClinicDoctor",
                newName: "ClinicsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DoctorPatient",
                table: "DoctorPatient",
                columns: new[] { "DoctorId", "PatientId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClinicDoctor",
                table: "ClinicDoctor",
                columns: new[] { "ClinicsId", "DoctorsId" });

            migrationBuilder.CreateIndex(
                name: "IX_DoctorPatient_PatientId",
                table: "DoctorPatient",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicDoctor_DoctorsId",
                table: "ClinicDoctor",
                column: "DoctorsId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClinicDoctor_Clinic_ClinicsId",
                table: "ClinicDoctor",
                column: "ClinicsId",
                principalTable: "Clinic",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClinicDoctor_Doctors_DoctorsId",
                table: "ClinicDoctor",
                column: "DoctorsId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorPatient_Doctors_DoctorId",
                table: "DoctorPatient",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorPatient_Patients_PatientId",
                table: "DoctorPatient",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
