using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RevaliInstruct.Core.Data
{
    /// <summary>
    /// Initialisatie van de database.
    ///
    /// BELANGRIJK:
    /// - Deze klasse seedt GEEN gebruikers/patiënten/oefeningen meer.
    /// - Alle domeindata moet nu uit Eindopdracht-data.xlsx komen
    ///   (via een import in SQL Server / SSMS).
    /// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// Volledige reset: database droppen (indien mogelijk) en opnieuw aanmaken.
        /// Wordt gebruikt door het dev endpoint of bij RESET_DB=1.
        /// LET OP: dit verwijdert ook alle Excel-data die je hebt geïmporteerd.
        /// </summary>
        public static async Task ResetAndSeedAsync(ApplicationDbContext context)
        {
            try
            {
                await context.Database.EnsureDeletedAsync();
                Console.WriteLine("Database dropped via EnsureDeletedAsync.");
            }
            catch (Exception ex)
            {
                // Bijv. als het SQL-account geen DROP/ALTER rechten heeft.
                Console.WriteLine($"Warning: failed to EnsureDeleted: {ex.Message}");
            }

            await EnsureSchemaAsync(context);
            await SeedAsync(context);
        }

        /// <summary>
        /// Zorgt dat het schema bestaat (migrations of EnsureCreated).
        /// </summary>
        private static async Task EnsureSchemaAsync(ApplicationDbContext context)
        {
            // Als je migrations gebruikt, vervang dit dan door:
            // await context.Database.MigrateAsync();

            await context.Database.EnsureCreatedAsync();
            Console.WriteLine("Database schema ensured (EnsureCreated/Migrate).");
        }

        /// <summary>
        /// Idempotente "seed".
        /// Doet GEEN inserts meer: alle data hoort nu uit de Excel-import te komen.
        /// </summary>
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            await EnsureSchemaAsync(context);

            // Geen hard-gecode data meer toevoegen:
            // - geen users doctor1/doctor2/admin
            // - geen demo-patiënten
            // - geen oefenassignments, pijnentries, afspraken, etc.
            //
            // De bedoeling is dat alle rijen in:
            //   - Users
            //   - Patients
            //   - Exercises
            //   - … enz.
            // rechtstreeks uit Eindopdracht-data.xlsx komen
            // (via een import in SQL Server / SSMS).

            var hasUsers = await context.Users.AnyAsync();
            if (!hasUsers)
            {
                Console.WriteLine(
                    "[DbInitializer] Waarschuwing: Users-tabel is leeg. " +
                    "Importeer de Excel-data (Eindopdracht-data.xlsx) in de database."
                );
            }
            else
            {
                Console.WriteLine(
                    "[DbInitializer] Users-tabel bevat al data – geen seed uitgevoerd."
                );
            }
        }
    }
}