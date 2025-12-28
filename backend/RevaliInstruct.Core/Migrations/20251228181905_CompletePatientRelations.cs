using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RevaliInstruct.Core.Migrations
{
    /// <inheritdoc />
    public partial class CompletePatientRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ExerciseAssignments_PatientId",
                table: "ExerciseAssignments",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_PatientId",
                table: "ActivityLogs",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessoryAdvices_PatientId",
                table: "AccessoryAdvices",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessoryAdvices_Patients_PatientId",
                table: "AccessoryAdvices",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLogs_Patients_PatientId",
                table: "ActivityLogs",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseAssignments_Patients_PatientId",
                table: "ExerciseAssignments",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessoryAdvices_Patients_PatientId",
                table: "AccessoryAdvices");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLogs_Patients_PatientId",
                table: "ActivityLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseAssignments_Patients_PatientId",
                table: "ExerciseAssignments");

            migrationBuilder.DropIndex(
                name: "IX_ExerciseAssignments_PatientId",
                table: "ExerciseAssignments");

            migrationBuilder.DropIndex(
                name: "IX_ActivityLogs_PatientId",
                table: "ActivityLogs");

            migrationBuilder.DropIndex(
                name: "IX_AccessoryAdvices_PatientId",
                table: "AccessoryAdvices");
        }
    }
}
