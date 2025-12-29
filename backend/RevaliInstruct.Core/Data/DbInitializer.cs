using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RevaliInstruct.Core.Data
{
    public static class DbInitializer
    {
        public static async Task ResetAndSeedAsync(ApplicationDbContext context)
        {
            try
            {
                // Volledige reset
                await context.Database.EnsureDeletedAsync();
                Console.WriteLine("[DbInitializer] Database verwijderd.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DbInitializer] Waarschuwing: verwijderen mislukt: {ex.Message}");
            }

            // Maak database aan en voer HasData uit de context uit
            await context.Database.EnsureCreatedAsync();
            Console.WriteLine("[DbInitializer] Database succesvol aangemaakt en gevuld met code-data.");
        }

        public static async Task SeedAsync(ApplicationDbContext context)
        {
            await context.Database.EnsureCreatedAsync();

            if (await context.Users.AnyAsync())
            {
                Console.WriteLine("[DbInitializer] Data is reeds aanwezig.");
            }
            else
            {
                Console.WriteLine("[DbInitializer] Database is leeg. Gebruik RESET_DB=1 voor een schone start.");
            }
        }
    }
}