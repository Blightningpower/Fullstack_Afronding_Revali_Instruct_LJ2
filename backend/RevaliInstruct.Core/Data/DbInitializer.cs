using System.Globalization;
using RevaliInstruct.Core.Entities;

namespace RevaliInstruct.Core.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext db)
        {
            if (db.Patients.Any()) return;

            // accepteer dd-MM-yyyy, dd/MM/yyyy en ddMMyyyy
            var dateFormats = new[] { "dd-MM-yyyy", "dd/MM/yyyy", "ddMMyyyy" };
            Func<string, DateTime> D = s => DateTime.ParseExact(s, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None);

            var patients = new[]
            {
                new Patient { FirstName = "Piet",  LastName = "Jansen",  StartDate = D("10-01-2025"), Status = PatientStatus.Actief },
                new Patient { FirstName = "Anna",  LastName = "De Vries",StartDate = D("02-03-2025"), Status = PatientStatus.Actief },
                new Patient { FirstName = "Karin", LastName = "Smit",    StartDate = D("20-11-2024"), Status = PatientStatus.Afgerond },
                new Patient { FirstName = "Bas",   LastName = "van Dijk",StartDate = D("16-01-2025"), Status = PatientStatus.OnHold },
                new Patient { FirstName = "Sofie", LastName = "Bakker",  StartDate = D("06-02-2025"), Status = PatientStatus.Actief },
                new Patient { FirstName = "Joris", LastName = "Kuipers", StartDate = D("03-03-2025"), Status = PatientStatus.Actief },
                new Patient { FirstName = "Mirella",LastName = "Bos",    StartDate = D("12-02-2025"), Status = PatientStatus.Actief },
                new Patient { FirstName = "Tom",   LastName = "Vogel",   StartDate = D("07-03-2025"), Status = PatientStatus.Actief },
                new Patient { FirstName = "Eline", LastName = "Klaassen",StartDate = D("03-04-2025"), Status = PatientStatus.Actief },
                new Patient { FirstName = "Mark",  LastName = "de Boer", StartDate = D("12-03-2025"), Status = PatientStatus.Afgerond }
            };

            db.Patients.AddRange(patients);
            await db.SaveChangesAsync();
        }
    }
}