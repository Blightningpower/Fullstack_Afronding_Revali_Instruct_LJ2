using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RevaliInstruct.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddRowLevelSecurity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Schema aanmaken (indien deze nog niet bestaat na de mislukte poging)
            migrationBuilder.Sql("IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Security') EXEC('CREATE SCHEMA Security')");

            //vPredicate Functie aanmaken
            migrationBuilder.Sql(@"
        CREATE OR ALTER FUNCTION Security.fn_patientAccessPredicate(@AssignedDoctorUserId int)
            RETURNS TABLE
            WITH SCHEMABINDING
        AS
            RETURN SELECT 1 AS fn_access_result
            WHERE CAST(SESSION_CONTEXT(N'UserId') AS int) = @AssignedDoctorUserId;");

            // Security Policy aanmaken (Gecorrigeerde syntax met herhaling van de tabelnaam)
            migrationBuilder.Sql(@"
        CREATE SECURITY POLICY Security.PatientFilter
            ADD FILTER PREDICATE Security.fn_patientAccessPredicate(AssignedDoctorUserId) ON dbo.Patients,
            ADD BLOCK PREDICATE Security.fn_patientAccessPredicate(AssignedDoctorUserId) ON dbo.Patients AFTER INSERT
            WITH (STATE = ON);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
