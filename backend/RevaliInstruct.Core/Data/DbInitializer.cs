using Microsoft.EntityFrameworkCore;

namespace RevaliInstruct.Core.Data
{
    public static class DbInitializer
    {
        public static async Task ResetAndSeedAsync(ApplicationDbContext context)
        {
            try
            {
                await context.Database.EnsureDeletedAsync();
                Console.WriteLine("[DbInitializer] Database verwijderd.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DbInitializer] Waarschuwing: verwijderen mislukt: {ex.Message}");
            }

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