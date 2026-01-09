using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RevaliInstruct.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddDatabaseTriggers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Trigger voor Declaraties: bedrag moet positief zijn
            migrationBuilder.Sql(@"
        CREATE TRIGGER TR_ValidateDeclarationAmount
        ON Declarations
        AFTER INSERT, UPDATE
        AS
        BEGIN
            IF EXISTS (SELECT 1 FROM inserted WHERE Amount <= 0)
            BEGIN
                RAISERROR ('Het bedrag van een declaratie moet groter zijn dan 0.', 16, 1);
                ROLLBACK TRANSACTION;
            END
        END");

            // Trigger voor Afspraken: datum mag niet in het verleden liggen
            migrationBuilder.Sql(@"
        CREATE TRIGGER TR_ValidateAppointmentDate
        ON Appointments
        AFTER INSERT, UPDATE
        AS
        BEGIN
            IF EXISTS (SELECT 1 FROM inserted WHERE DateTime < GETDATE() AND Status = 'Gepland')
            BEGIN
                RAISERROR ('Een nieuwe afspraak kan niet in het verleden worden gepland.', 16, 1);
                ROLLBACK TRANSACTION;
            END
        END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
