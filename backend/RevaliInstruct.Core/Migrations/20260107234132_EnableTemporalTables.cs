using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RevaliInstruct.Core.Migrations
{
    /// <inheritdoc />
    public partial class EnableTemporalTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Goals",
                table: "IntakeRecords",
                newName: "InitialGoals");

            migrationBuilder.AlterTable(
                name: "Patients")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "PatientsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.AlterTable(
                name: "Medication")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "MedicationHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.AlterTable(
                name: "Declarations")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DeclarationsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.AlterTable(
                name: "Appointments")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "AppointmentsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Patients",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Patients",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Patients",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PeriodEnd",
                table: "Patients",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PeriodStart",
                table: "Patients",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "PatientNotes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PeriodEnd",
                table: "Medication",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PeriodStart",
                table: "Medication",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PeriodEnd",
                table: "Declarations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PeriodStart",
                table: "Declarations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.AddColumn<int>(
                name: "RecordId",
                table: "AuditLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TableName",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "PeriodEnd",
                table: "Appointments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PeriodStart",
                table: "Appointments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.InsertData(
                table: "Declarations",
                columns: new[] { "Id", "Amount", "Date", "DoctorId", "PatientId", "Status", "TreatmentType" },
                values: new object[,]
                {
                    { 27, 8000m, new DateTime(2025, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 1, "Geregistreerd", "Intake revalidatie knie" },
                    { 28, 6000m, new DateTime(2025, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 1, "Geregistreerd", "Controle consult 1" },
                    { 29, 6000m, new DateTime(2025, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 1, "Geregistreerd", "Controle consult 2" },
                    { 30, 6000m, new DateTime(2025, 3, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 1, "Geregistreerd", "Controle consult 3" },
                    { 31, 6000m, new DateTime(2025, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 1, "Geregistreerd", "Controle consult 4" },
                    { 32, 8000m, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 2, "Geregistreerd", "Intake revalidatie rug" },
                    { 33, 6000m, new DateTime(2025, 2, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 2, "Geregistreerd", "Controle consult 1" },
                    { 34, 6000m, new DateTime(2025, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 2, "Geregistreerd", "Controle consult 2" },
                    { 35, 8000m, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 3, "Geregistreerd", "Intake revalidatie schouder" },
                    { 36, 6000m, new DateTime(2025, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 3, "Geregistreerd", "Controle consult 1" },
                    { 37, 8000m, new DateTime(2025, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 4, "Geregistreerd", "Intake revalidatie enkel" },
                    { 38, 6000m, new DateTime(2025, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 4, "Geregistreerd", "Controle consult 1" },
                    { 39, 8000m, new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 5, "Geregistreerd", "Intake revalidatie nek" },
                    { 40, 6000m, new DateTime(2025, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 5, "Geregistreerd", "Controle consult 1" },
                    { 41, 8000m, new DateTime(2025, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 6, "Geregistreerd", "Intake chronische rugpijn" },
                    { 42, 6000m, new DateTime(2025, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 6, "Geregistreerd", "Controle consult 1" },
                    { 43, 8000m, new DateTime(2025, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 7, "Geregistreerd", "Intake meniscus" },
                    { 44, 6000m, new DateTime(2025, 3, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 7, "Geregistreerd", "Controle consult 1" },
                    { 45, 8000m, new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 8, "Geregistreerd", "Intake fibromyalgie" },
                    { 46, 6000m, new DateTime(2025, 4, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 8, "Geregistreerd", "Controle consult 1" },
                    { 47, 8000m, new DateTime(2025, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 9, "Geregistreerd", "Intake CTS" },
                    { 48, 6000m, new DateTime(2025, 3, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 9, "Geregistreerd", "Controle consult 1" },
                    { 49, 8000m, new DateTime(2025, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 10, "Geregistreerd", "Intake knie artrose" },
                    { 50, 6000m, new DateTime(2025, 4, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 10, "Geregistreerd", "Controle consult 1" },
                    { 51, 8000m, new DateTime(2025, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 11, "Geregistreerd", "Intake algemeen" },
                    { 52, 6000m, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 11, "Geregistreerd", "Controle consult 1" },
                    { 53, 8000m, new DateTime(2025, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 12, "Geregistreerd", "Intake revalidatie" },
                    { 54, 6000m, new DateTime(2025, 5, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 12, "Geregistreerd", "Controle consult 1" },
                    { 55, 8000m, new DateTime(2025, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 13, "Geregistreerd", "Intake oudere patiënt" },
                    { 56, 6000m, new DateTime(2025, 5, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 13, "Geregistreerd", "Controle consult 1" },
                    { 57, 8000m, new DateTime(2025, 4, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 14, "Geregistreerd", "Intake diverse klachten" },
                    { 58, 6000m, new DateTime(2025, 5, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 14, "Geregistreerd", "Controle consult 1" },
                    { 59, 8000m, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 15, "Geregistreerd", "Intake nieuwe patiënt" },
                    { 60, 6000m, new DateTime(2025, 5, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 15, "Geregistreerd", "Controle consult 1" },
                    { 61, 8000m, new DateTime(2025, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 16, "Geregistreerd", "Intake chronische pijn" },
                    { 62, 6000m, new DateTime(2025, 5, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 16, "Geregistreerd", "Controle consult 1" },
                    { 63, 8000m, new DateTime(2025, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 17, "Geregistreerd", "Intake post-op" },
                    { 64, 6000m, new DateTime(2025, 5, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 17, "Geregistreerd", "Controle consult 1" },
                    { 65, 8000m, new DateTime(2025, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 18, "Geregistreerd", "Intake complexe case" },
                    { 66, 6000m, new DateTime(2025, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 18, "Geregistreerd", "Controle consult 1" },
                    { 67, 8000m, new DateTime(2025, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 19, "Geregistreerd", "Intake sportblessure" },
                    { 68, 6000m, new DateTime(2025, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 19, "Geregistreerd", "Controle consult 1" },
                    { 69, 8000m, new DateTime(2025, 5, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 20, "Geregistreerd", "Intake revalidatieplan" },
                    { 70, 6000m, new DateTime(2025, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 20, "Geregistreerd", "Controle consult 1" },
                    { 71, 8000m, new DateTime(2025, 5, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 21, "Geregistreerd", "Intake algemeen herstel" },
                    { 72, 6000m, new DateTime(2025, 6, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 21, "Geregistreerd", "Controle consult 1" },
                    { 73, 8000m, new DateTime(2025, 5, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 22, "Geregistreerd", "Intake chronische aandoening" },
                    { 74, 6000m, new DateTime(2025, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 22, "Geregistreerd", "Controle consult 1" },
                    { 75, 8000m, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 23, "Geregistreerd", "Intake nieuwe doorverwijzing" },
                    { 76, 6000m, new DateTime(2025, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 23, "Geregistreerd", "Controle consult 1" },
                    { 77, 8000m, new DateTime(2025, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 24, "Geregistreerd", "Intake breukherstel" },
                    { 78, 6000m, new DateTime(2025, 6, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 24, "Geregistreerd", "Controle consult 1" },
                    { 79, 8000m, new DateTime(2025, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 25, "Geregistreerd", "Intake algemeen traject" },
                    { 80, 6000m, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 25, "Geregistreerd", "Controle consult 1" }
                });

            migrationBuilder.UpdateData(
                table: "PatientNotes",
                keyColumn: "Id",
                keyValue: 1,
                column: "AuthorId",
                value: null);

            migrationBuilder.UpdateData(
                table: "PatientNotes",
                keyColumn: "Id",
                keyValue: 2,
                column: "AuthorId",
                value: null);

            migrationBuilder.UpdateData(
                table: "PatientNotes",
                keyColumn: "Id",
                keyValue: 3,
                column: "AuthorId",
                value: null);

            migrationBuilder.UpdateData(
                table: "PatientNotes",
                keyColumn: "Id",
                keyValue: 4,
                column: "AuthorId",
                value: null);

            migrationBuilder.UpdateData(
                table: "PatientNotes",
                keyColumn: "Id",
                keyValue: 5,
                column: "AuthorId",
                value: null);

            migrationBuilder.UpdateData(
                table: "PatientNotes",
                keyColumn: "Id",
                keyValue: 6,
                column: "AuthorId",
                value: null);

            migrationBuilder.UpdateData(
                table: "PatientNotes",
                keyColumn: "Id",
                keyValue: 7,
                column: "AuthorId",
                value: null);

            migrationBuilder.UpdateData(
                table: "PatientNotes",
                keyColumn: "Id",
                keyValue: 8,
                column: "AuthorId",
                value: null);

            migrationBuilder.UpdateData(
                table: "PatientNotes",
                keyColumn: "Id",
                keyValue: 9,
                column: "AuthorId",
                value: null);

            migrationBuilder.UpdateData(
                table: "PatientNotes",
                keyColumn: "Id",
                keyValue: 10,
                column: "AuthorId",
                value: null);

            migrationBuilder.UpdateData(
                table: "PatientNotes",
                keyColumn: "Id",
                keyValue: 11,
                column: "AuthorId",
                value: null);

            migrationBuilder.UpdateData(
                table: "PatientNotes",
                keyColumn: "Id",
                keyValue: 12,
                column: "AuthorId",
                value: null);

            migrationBuilder.UpdateData(
                table: "PatientNotes",
                keyColumn: "Id",
                keyValue: 13,
                column: "AuthorId",
                value: null);

            migrationBuilder.UpdateData(
                table: "PatientNotes",
                keyColumn: "Id",
                keyValue: 14,
                column: "AuthorId",
                value: null);

            migrationBuilder.UpdateData(
                table: "PatientNotes",
                keyColumn: "Id",
                keyValue: 15,
                column: "AuthorId",
                value: null);

            migrationBuilder.UpdateData(
                table: "PatientNotes",
                keyColumn: "Id",
                keyValue: 16,
                column: "AuthorId",
                value: null);

            migrationBuilder.UpdateData(
                table: "PatientNotes",
                keyColumn: "Id",
                keyValue: 17,
                column: "AuthorId",
                value: null);

            migrationBuilder.UpdateData(
                table: "PatientNotes",
                keyColumn: "Id",
                keyValue: 18,
                column: "AuthorId",
                value: null);

            migrationBuilder.UpdateData(
                table: "PatientNotes",
                keyColumn: "Id",
                keyValue: 19,
                column: "AuthorId",
                value: null);

            migrationBuilder.UpdateData(
                table: "PatientNotes",
                keyColumn: "Id",
                keyValue: 20,
                column: "AuthorId",
                value: null);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "FirstName", "LastName", "OrganisatieId", "PasswordHash", "Role", "Username" },
                values: new object[] { 8, "Admin", "User", null, "$2y$10$9mIxp6HbC1KobigZYb0qUu98xe11w45XH1kvPKX2qZ44BsF2qObDy", "Admin", "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Patients_Email",
                table: "Patients",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PatientNotes_AuthorId",
                table: "PatientNotes",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientNotes_Users_AuthorId",
                table: "PatientNotes",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientNotes_Users_AuthorId",
                table: "PatientNotes");

            migrationBuilder.DropIndex(
                name: "IX_Patients_Email",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_PatientNotes_AuthorId",
                table: "PatientNotes");

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "Declarations",
                keyColumn: "Id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DropColumn(
                name: "PeriodEnd",
                table: "Patients")
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.DropColumn(
                name: "PeriodStart",
                table: "Patients")
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "PatientNotes");

            migrationBuilder.DropColumn(
                name: "PeriodEnd",
                table: "Medication")
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.DropColumn(
                name: "PeriodStart",
                table: "Medication")
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.DropColumn(
                name: "PeriodEnd",
                table: "Declarations")
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.DropColumn(
                name: "PeriodStart",
                table: "Declarations")
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.DropColumn(
                name: "RecordId",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "TableName",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "PeriodEnd",
                table: "Appointments")
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.DropColumn(
                name: "PeriodStart",
                table: "Appointments")
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.RenameColumn(
                name: "InitialGoals",
                table: "IntakeRecords",
                newName: "Goals");

            migrationBuilder.AlterTable(
                name: "Patients")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "PatientsHistory")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.AlterTable(
                name: "Medication")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "MedicationHistory")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.AlterTable(
                name: "Declarations")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DeclarationsHistory")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.AlterTable(
                name: "Appointments")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "AppointmentsHistory")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
