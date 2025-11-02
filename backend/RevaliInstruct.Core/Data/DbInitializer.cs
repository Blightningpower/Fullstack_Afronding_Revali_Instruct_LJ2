using System.Globalization;
using RevaliInstruct.Core.Entities;   

namespace RevaliInstruct.Core.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            // Als er al patients zijn: niets doen
            if (db.Patients.Any()) return;

            // ondersteunde datumnotaties (dd-MM-yyyy en d-M-yyyy)
            var dateFormats = new[] { "dd-MM-yyyy", "d-M-yyyy" };
            DateTime ParseD(string s) => DateTime.ParseExact(s, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None);

            var patients = new[]
            {
                new Patient { FirstName = "Piet",    LastName = "Jansen",   StartDate = ParseD("10-01-2025"), Status = PatientStatus.Actief,   CreatedAt = DateTime.UtcNow },
                new Patient { FirstName = "Anna",    LastName = "de Vries", StartDate = ParseD("02-03-2025"), Status = PatientStatus.Actief,   CreatedAt = DateTime.UtcNow },
                new Patient { FirstName = "Karin",   LastName = "Smit",     StartDate = ParseD("20-11-2024"), Status = PatientStatus.Afgerond, CreatedAt = DateTime.UtcNow },
                new Patient { FirstName = "Bas",     LastName = "van Dijk", StartDate = ParseD("16-01-2025"), Status = PatientStatus.OnHold,   CreatedAt = DateTime.UtcNow },
                new Patient { FirstName = "Sofie",   LastName = "Bakker",   StartDate = ParseD("06-02-2025"), Status = PatientStatus.Actief,   CreatedAt = DateTime.UtcNow },
                new Patient { FirstName = "Joris",   LastName = "Kuipers",  StartDate = ParseD("03-03-2025"), Status = PatientStatus.Actief,   CreatedAt = DateTime.UtcNow },
                new Patient { FirstName = "Mirella", LastName = "Bos",      StartDate = ParseD("12-02-2025"), Status = PatientStatus.Actief,   CreatedAt = DateTime.UtcNow },
                new Patient { FirstName = "Tom",     LastName = "Vogel",    StartDate = ParseD("07-03-2025"), Status = PatientStatus.Actief,   CreatedAt = DateTime.UtcNow },
                new Patient { FirstName = "Eline",   LastName = "Klaassen", StartDate = ParseD("03-04-2025"), Status = PatientStatus.Actief,   CreatedAt = DateTime.UtcNow },
                new Patient { FirstName = "Mark",    LastName = "de Boer",  StartDate = ParseD("12-03-2025"), Status = PatientStatus.Afgerond, CreatedAt = DateTime.UtcNow }
            };

            db.Patients.AddRange(patients);
            await db.SaveChangesAsync();
        }
    }
}