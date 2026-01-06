using Microsoft.EntityFrameworkCore;
using RevaliInstruct.Core.Entities;
using RevaliInstruct.Core.Data.Enums;

namespace RevaliInstruct.Core.Data
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            var staticHash = "$2y$10$9mIxp6HbC1KobigZYb0qUu98xe11w45XH1kvPKX2qZ44BsF2qObDy";

            // 1. Gebruikers
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "zvm_jansen", PasswordHash = staticHash, Role = "Zorgverzekeraar", FirstName = "Sophie", LastName = "Jansen", OrganisatieId = 1 },
                new User { Id = 2, Username = "zvm_devries", PasswordHash = staticHash, Role = "Zorgverzekeraar", FirstName = "Mark", LastName = "de Vries", OrganisatieId = 2 },
                new User { Id = 3, Username = "ha_bakker", PasswordHash = staticHash, Role = "Huisarts", FirstName = "Lisa", LastName = "Bakker" },
                new User { Id = 4, Username = "ha_janssen", PasswordHash = staticHash, Role = "Huisarts", FirstName = "Thomas", LastName = "Janssen" },
                new User { Id = 5, Username = "ra_smit", PasswordHash = staticHash, Role = "Revalidatiearts", FirstName = "Emma", LastName = "Smit" },
                new User { Id = 6, Username = "ra_groen", PasswordHash = staticHash, Role = "Revalidatiearts", FirstName = "David", LastName = "Groen" },
                new User { Id = 7, Username = "ra_visser", PasswordHash = staticHash, Role = "Revalidatiearts", FirstName = "Linda", LastName = "Visser" },
                new User { Id = 8, Username = "admin", PasswordHash = staticHash, Role = "Admin", FirstName = "Admin", LastName = "User" }
            );

            // 2. Patienten
            modelBuilder.Entity<Patient>().HasData(
                 new Patient { Id = 1, FirstName = "Freddy", LastName = "Voetballer", BirthDate = new DateTime(1995, 3, 15), Address = "Kruisbandstraat 10", Phone = "0612345678", Email = "freddy.v@example.com", AssignedDoctorUserId = 5, ReferringDoctorUserId = 3, StartDate = new DateTime(2025, 1, 12), Status = PatientStatus.Active },
                 new Patient { Id = 2, FirstName = "Anna", LastName = "Kamer", BirthDate = new DateTime(1988, 11, 22), Address = "Therapiestraat 5", Phone = "0623456789", Email = "anna.k@example.com", AssignedDoctorUserId = 6, ReferringDoctorUserId = 4, StartDate = new DateTime(2025, 2, 3), Status = PatientStatus.Completed },
                 new Patient { Id = 3, FirstName = "Bas", LastName = "Verkade", BirthDate = new DateTime(1976, 7, 1), Address = "Oefenlaan 12", Phone = "0634567890", Email = "bas.v@example.com", AssignedDoctorUserId = 5, ReferringDoctorUserId = 3, StartDate = new DateTime(2025, 1, 17), Status = PatientStatus.Active },
                 new Patient { Id = 4, FirstName = "Carla", LastName = "Dirksen", BirthDate = new DateTime(2001, 1, 30), Address = "Herstelweg 3", Phone = "0645678901", Email = "carla.d@example.com", AssignedDoctorUserId = 7, ReferringDoctorUserId = 4, StartDate = new DateTime(2025, 4, 1), Status = PatientStatus.IntakePlanned },
                 new Patient { Id = 5, FirstName = "Dirk", LastName = "Meijer", BirthDate = new DateTime(1965, 9, 10), Address = "Revalidatieplein 7", Phone = "0656789012", Email = "dirk.m@example.com", AssignedDoctorUserId = 6, ReferringDoctorUserId = 3, StartDate = new DateTime(2025, 2, 7), Status = PatientStatus.OnHold },
                 new Patient { Id = 6, FirstName = "Eva", LastName = "Peters", BirthDate = new DateTime(1999, 4, 5), Address = "Fysioweg 22", Phone = "0667890123", Email = "eva.p@example.com", AssignedDoctorUserId = 5, ReferringDoctorUserId = 4, StartDate = new DateTime(2025, 2, 12), Status = PatientStatus.Active },
                 new Patient { Id = 7, FirstName = "Frank", LastName = "de Jong", BirthDate = new DateTime(1970, 2, 18), Address = "Gipsstraat 1", Phone = "0678901234", Email = "frank.dj@example.com", AssignedDoctorUserId = 7, ReferringDoctorUserId = 3, StartDate = new DateTime(2025, 3, 7), Status = PatientStatus.IntakePlanned },
                 new Patient { Id = 8, FirstName = "Greetje", LastName = "Willems", BirthDate = new DateTime(1985, 12, 3), Address = "Rolstoelpad 8", Phone = "0689012345", Email = "greetje.w@example.com", AssignedDoctorUserId = 6, ReferringDoctorUserId = 4, StartDate = new DateTime(2025, 3, 4), Status = PatientStatus.Active },
                 new Patient { Id = 9, FirstName = "Hans", LastName = "Kramer", BirthDate = new DateTime(1993, 6, 25), Address = "Sportlaan 15", Phone = "0690123456", Email = "hans.k@example.com", AssignedDoctorUserId = 5, ReferringDoctorUserId = 3, StartDate = new DateTime(2025, 3, 12), Status = PatientStatus.Completed },
                 new Patient { Id = 10, FirstName = "Irene", LastName = "Van Dam", BirthDate = new DateTime(1972, 10, 14), Address = "Zorgcentrum 4", Phone = "0601234567", Email = "irene.vd@example.com", AssignedDoctorUserId = 7, ReferringDoctorUserId = 4, StartDate = new DateTime(2025, 4, 7), Status = PatientStatus.IntakePlanned },
                 new Patient { Id = 11, FirstName = "Jeroen", LastName = "Faber", BirthDate = new DateTime(1980, 8, 8), Address = "Gezondheidsstraat 2", Phone = "0611223344", Email = "jeroen.f@example.com", AssignedDoctorUserId = 6, ReferringDoctorUserId = 3, StartDate = new DateTime(2025, 4, 10), Status = PatientStatus.Active },
                 new Patient { Id = 12, FirstName = "Kim", LastName = "Schouten", BirthDate = new DateTime(1997, 1, 20), Address = "Pijnbestrijding 1", Phone = "0622334455", Email = "kim.s@example.com", AssignedDoctorUserId = 5, ReferringDoctorUserId = 4, StartDate = new DateTime(2025, 4, 15), Status = PatientStatus.OnHold },
                 new Patient { Id = 13, FirstName = "Lars", LastName = "Veenstra", BirthDate = new DateTime(1960, 5, 12), Address = "Welzijnshof 9", Phone = "0633445566", Email = "lars.v@example.com", AssignedDoctorUserId = 7, ReferringDoctorUserId = 3, StartDate = new DateTime(2025, 4, 20), Status = PatientStatus.Active },
                 new Patient { Id = 14, FirstName = "Mieke", LastName = "Bos", BirthDate = new DateTime(1990, 3, 3), Address = "Bewegingstraat 6", Phone = "0644556677", Email = "mieke.b@example.com", AssignedDoctorUserId = 6, ReferringDoctorUserId = 4, StartDate = new DateTime(2025, 4, 25), Status = PatientStatus.IntakePlanned },
                 new Patient { Id = 15, FirstName = "Niels", LastName = "Dekker", BirthDate = new DateTime(1978, 7, 28), Address = "Vitaliteitslaan 11", Phone = "0655667788", Email = "niels.d@example.com", AssignedDoctorUserId = 5, ReferringDoctorUserId = 3, StartDate = new DateTime(2025, 5, 1), Status = PatientStatus.Active },
                 new Patient { Id = 16, FirstName = "Olga", LastName = "Postma", BirthDate = new DateTime(1983, 9, 9), Address = "Herstelstraat 14", Phone = "0666778899", Email = "olga.p@example.com", AssignedDoctorUserId = 7, ReferringDoctorUserId = 4, StartDate = new DateTime(2025, 5, 5), Status = PatientStatus.Completed },
                 new Patient { Id = 17, FirstName = "Paul", LastName = "Kuipers", BirthDate = new DateTime(1992, 11, 11), Address = "Zorgpad 1", Phone = "0677889900", Email = "paul.k@example.com", AssignedDoctorUserId = 6, ReferringDoctorUserId = 3, StartDate = new DateTime(2025, 5, 10), Status = PatientStatus.Active },
                 new Patient { Id = 18, FirstName = "Quinn", LastName = "Vries", BirthDate = new DateTime(1975, 4, 4), Address = "Medicijnlaan 3", Phone = "0688990011", Email = "quinn.v@example.com", AssignedDoctorUserId = 5, ReferringDoctorUserId = 4, StartDate = new DateTime(2025, 5, 15), Status = PatientStatus.IntakePlanned },
                 new Patient { Id = 19, FirstName = "Ruben", LastName = "Smeets", BirthDate = new DateTime(1987, 2, 2), Address = "Oefenplein 5", Phone = "0699001122", Email = "ruben.s@example.com", AssignedDoctorUserId = 7, ReferringDoctorUserId = 3, StartDate = new DateTime(2025, 5, 20), Status = PatientStatus.OnHold },
                 new Patient { Id = 20, FirstName = "Sarah", LastName = "Koks", BirthDate = new DateTime(1994, 8, 18), Address = "Genezingsweg 10", Phone = "0610203040", Email = "sarah.k@example.com", AssignedDoctorUserId = 6, ReferringDoctorUserId = 4, StartDate = new DateTime(2025, 5, 25), Status = PatientStatus.Active },
                 new Patient { Id = 21, FirstName = "Tim", LastName = "Visser", BirthDate = new DateTime(1968, 10, 25), Address = "Sportlaan 2", Phone = "0620304050", Email = "tim.v@example.com", AssignedDoctorUserId = 5, ReferringDoctorUserId = 3, StartDate = new DateTime(2025, 5, 28), Status = PatientStatus.Completed },
                 new Patient { Id = 22, FirstName = "Ursula", LastName = "Bouwman", BirthDate = new DateTime(1981, 6, 7), Address = "Kwaliteitsstraat 7", Phone = "0630405060", Email = "ursula.b@example.com", AssignedDoctorUserId = 7, ReferringDoctorUserId = 4, StartDate = new DateTime(2025, 5, 30), Status = PatientStatus.Active },
                 new Patient { Id = 23, FirstName = "Vincent", LastName = "Meijerink", BirthDate = new DateTime(1996, 3, 29), Address = "Zorglaan 9", Phone = "0640506070", Email = "vincent.m@example.com", AssignedDoctorUserId = 6, ReferringDoctorUserId = 3, StartDate = new DateTime(2025, 6, 1), Status = PatientStatus.IntakePlanned },
                 new Patient { Id = 24, FirstName = "Wendy", LastName = "van der Velde", BirthDate = new DateTime(1979, 1, 13), Address = "Therapieplein 12", Phone = "0650607080", Email = "wendy.vdv@example.com", AssignedDoctorUserId = 5, ReferringDoctorUserId = 4, StartDate = new DateTime(2025, 6, 5), Status = PatientStatus.Active },
                 new Patient { Id = 25, FirstName = "Xavier", LastName = "De Ruiter", BirthDate = new DateTime(1986, 5, 5), Address = "Revalidatiepad 15", Phone = "0660708090", Email = "xavier.dr@example.com", AssignedDoctorUserId = 7, ReferringDoctorUserId = 3, StartDate = new DateTime(2025, 6, 10), Status = PatientStatus.Completed }
             );

            // 3. Oefeningen
            modelBuilder.Entity<Exercise>().HasData(
                new Exercise { Id = 1, Name = "Kniebuiging", Description = "Langzaam knieën buigen tot 90 graden rug recht houden.", VideoUrl = "https://www.youtube.com/watch?v=00g8DINXtIY" },
                new Exercise { Id = 2, Name = "Hamstring Stretch", Description = "Zittend één been gestrekt", VideoUrl = "https://www.youtube.com/shorts/ywgDXzYD6v8" },
                new Exercise { Id = 3, Name = "Calf Raises", Description = "Op de tenen staan en langzaam zakken.", VideoUrl = "https://www.youtube.com/watch?v=eMTy3qylqnE" },
                new Exercise { Id = 4, Name = "Core Stability Plank", Description = "Plankpositie rug recht", VideoUrl = "https://www.youtube.com/watch?v=pvIjsG5Svck" },
                new Exercise { Id = 5, Name = "Schouder Rotaties", Description = "Cirkelbewegingen met de armen.", VideoUrl = "https://www.youtube.com/watch?v=oLwTC-lAJws" }
            );

            // 4. Oefenplannen
            modelBuilder.Entity<ExerciseAssignment>().HasData(
                new ExerciseAssignment { Id = 1, PatientId = 1, ExerciseId = 1, StartDate = new DateTime(2025, 1, 12), EndDate = new DateTime(2025, 3, 1), Repetitions = 10, Sets = 3, Frequency = 3, Notes = "Focus op stabiliteit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 13, 18, 0, 0) },
                new ExerciseAssignment { Id = 2, PatientId = 1, ExerciseId = 2, StartDate = new DateTime(2025, 1, 12), EndDate = new DateTime(2025, 3, 1), Repetitions = 15, Sets = 3, Frequency = 3, Notes = "Hamstring flexibiliteit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 13, 9, 15, 0) },
                new ExerciseAssignment { Id = 3, PatientId = 1, ExerciseId = 3, StartDate = new DateTime(2025, 1, 12), EndDate = new DateTime(2025, 3, 1), Repetitions = 12, Sets = 3, Frequency = 3, Notes = "Kuitspierversterking", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 14, 9, 30, 0) },
                new ExerciseAssignment { Id = 4, PatientId = 1, ExerciseId = 4, StartDate = new DateTime(2025, 1, 20), EndDate = new DateTime(2025, 4, 1), Repetitions = 1, Sets = 3, Frequency = 1, Notes = "Houd 30 sec vast", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 23, 10, 0, 0) },
                new ExerciseAssignment { Id = 5, PatientId = 1, ExerciseId = 5, StartDate = new DateTime(2025, 2, 3), EndDate = new DateTime(2025, 5, 1), Repetitions = 12, Sets = 4, Frequency = 2, Notes = "Verhoging intensiteit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 3, 3, 9, 0, 0) },
                new ExerciseAssignment { Id = 6, PatientId = 1, ExerciseId = 2, StartDate = new DateTime(2025, 2, 3), EndDate = new DateTime(2025, 5, 1), Repetitions = 20, Sets = 3, Frequency = 3, Notes = "Verdieping stretch", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 3, 3, 18, 0, 0) },
                new ExerciseAssignment { Id = 7, PatientId = 1, ExerciseId = 5, StartDate = new DateTime(2025, 3, 10), EndDate = new DateTime(2025, 5, 15), Repetitions = 10, Sets = 3, Frequency = 3, Notes = "Lichte schouderoefeningen voor algehele conditie", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 3, 3, 9, 15, 0) },
                new ExerciseAssignment { Id = 8, PatientId = 1, ExerciseId = 3, StartDate = new DateTime(2025, 2, 5), EndDate = new DateTime(2025, 7, 1), Repetitions = 15, Sets = 4, Frequency = 2, Notes = "Laatste fase knieherstel", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 3, 5, 9, 0, 0) },
                new ExerciseAssignment { Id = 9, PatientId = 1, ExerciseId = 4, StartDate = new DateTime(2025, 4, 5), EndDate = new DateTime(2025, 6, 1), Repetitions = 1, Sets = 4, Frequency = 1, Notes = "Core versterking gevorderd", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 4, 6, 9, 0, 0) },
                new ExerciseAssignment { Id = 10, PatientId = 1, ExerciseId = 3, StartDate = new DateTime(2025, 4, 10), EndDate = new DateTime(2025, 6, 15), Repetitions = 15, Sets = 3, Frequency = 3, Notes = "Meer explosieve kuitbewegingen", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 4, 11, 9, 0, 0) },
                new ExerciseAssignment { Id = 11, PatientId = 2, ExerciseId = 1, StartDate = new DateTime(2025, 3, 2), EndDate = new DateTime(2025, 4, 1), Repetitions = 8, Sets = 3, Frequency = 3, Notes = "Voorzichtige beweging", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 4, 9, 0, 0) },
                new ExerciseAssignment { Id = 12, PatientId = 2, ExerciseId = 4, StartDate = new DateTime(2025, 3, 2), EndDate = new DateTime(2025, 4, 1), Repetitions = 1, Sets = 2, Frequency = 2, Notes = "Houd 20 sec vast", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 4, 10, 0, 0) },
                new ExerciseAssignment { Id = 13, PatientId = 2, ExerciseId = 2, StartDate = new DateTime(2025, 1, 3), EndDate = new DateTime(2025, 5, 1), Repetitions = 10, Sets = 3, Frequency = 1, Notes = "Verbeteren flexibiliteit onderrug", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 3, 2, 9, 0, 0) },
                new ExerciseAssignment { Id = 14, PatientId = 2, ExerciseId = 1, StartDate = new DateTime(2025, 2, 4), EndDate = new DateTime(2025, 6, 1), Repetitions = 10, Sets = 3, Frequency = 2, Notes = "Verhoging belasting", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 4, 3, 9, 0, 0) },
                new ExerciseAssignment { Id = 15, PatientId = 2, ExerciseId = 3, StartDate = new DateTime(2025, 4, 15), EndDate = new DateTime(2025, 6, 30), Repetitions = 10, Sets = 2, Frequency = 2, Notes = "Enkelstabiliteit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 4, 16, 9, 0, 0) },
                new ExerciseAssignment { Id = 16, PatientId = 3, ExerciseId = 5, StartDate = new DateTime(2025, 1, 17), EndDate = new DateTime(2025, 3, 1), Repetitions = 15, Sets = 3, Frequency = 3, Notes = "Focus op rotatie", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 18, 9, 0, 0) },
                new ExerciseAssignment { Id = 17, PatientId = 3, ExerciseId = 1, StartDate = new DateTime(2025, 1, 2), EndDate = new DateTime(2025, 4, 1), Repetitions = 10, Sets = 2, Frequency = 1, Notes = "Algemene conditie", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 2, 9, 0, 0) },
                new ExerciseAssignment { Id = 18, PatientId = 3, ExerciseId = 5, StartDate = new DateTime(2025, 3, 5), EndDate = new DateTime(2025, 5, 1), Repetitions = 12, Sets = 3, Frequency = 2, Notes = "Schouderflexibiliteit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 3, 6, 9, 0, 0) },
                new ExerciseAssignment { Id = 19, PatientId = 4, ExerciseId = 4, StartDate = new DateTime(2025, 1, 4), EndDate = new DateTime(2025, 6, 1), Repetitions = 1, Sets = 3, Frequency = 1, Notes = "Core stabiliteit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 4, 2, 9, 0, 0) },
                new ExerciseAssignment { Id = 20, PatientId = 4, ExerciseId = 3, StartDate = new DateTime(2025, 2, 7), EndDate = new DateTime(2025, 4, 1), Repetitions = 12, Sets = 3, Frequency = 2, Notes = "Enkelversterking", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 8, 9, 0, 0) },
                new ExerciseAssignment { Id = 21, PatientId = 4, ExerciseId = 1, StartDate = new DateTime(2025, 2, 7), EndDate = new DateTime(2025, 4, 1), Repetitions = 8, Sets = 2, Frequency = 1, Notes = "Lichte knieoefeningen ter ondersteuning", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 8, 10, 0, 0) },
                new ExerciseAssignment { Id = 22, PatientId = 4, ExerciseId = 2, StartDate = new DateTime(2025, 2, 4), EndDate = new DateTime(2025, 6, 1), Repetitions = 10, Sets = 3, Frequency = 3, Notes = "Hamstring stretch voor balans", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 4, 3, 9, 0, 0) },
                new ExerciseAssignment { Id = 23, PatientId = 4, ExerciseId = 5, StartDate = new DateTime(2025, 4, 10), EndDate = new DateTime(2025, 6, 30), Repetitions = 10, Sets = 3, Frequency = 1, Notes = "Schouder mobiliteit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 4, 11, 9, 0, 0) },
                new ExerciseAssignment { Id = 24, PatientId = 5, ExerciseId = 1, StartDate = new DateTime(2025, 3, 3), EndDate = new DateTime(2025, 5, 1), Repetitions = 8, Sets = 3, Frequency = 2, Notes = "Nek- en rugoefeningen", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 3, 4, 9, 0, 0) },
                new ExerciseAssignment { Id = 25, PatientId = 5, ExerciseId = 4, StartDate = new DateTime(2025, 3, 3), EndDate = new DateTime(2025, 5, 1), Repetitions = 1, Sets = 2, Frequency = 1, Notes = "Nekstabiliteit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 3, 4, 10, 0, 0) },
                new ExerciseAssignment { Id = 26, PatientId = 5, ExerciseId = 2, StartDate = new DateTime(2025, 4, 5), EndDate = new DateTime(2025, 6, 1), Repetitions = 10, Sets = 3, Frequency = 1, Notes = "Arm- en schouderstretch", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 4, 6, 9, 0, 0) },
                new ExerciseAssignment { Id = 27, PatientId = 6, ExerciseId = 1, StartDate = new DateTime(2025, 2, 12), EndDate = new DateTime(2025, 4, 1), Repetitions = 10, Sets = 3, Frequency = 2, Notes = "Rugversterking", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 13, 9, 0, 0) },
                new ExerciseAssignment { Id = 28, PatientId = 6, ExerciseId = 2, StartDate = new DateTime(2025, 2, 12), EndDate = new DateTime(2025, 4, 1), Repetitions = 10, Sets = 3, Frequency = 1, Notes = "Onderrug stretch", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 13, 10, 0, 0) },
                new ExerciseAssignment { Id = 29, PatientId = 6, ExerciseId = 4, StartDate = new DateTime(2025, 3, 1), EndDate = new DateTime(2025, 5, 1), Repetitions = 1, Sets = 3, Frequency = 1, Notes = "Core voor rug", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 3, 2, 9, 0, 0) },
                new ExerciseAssignment { Id = 30, PatientId = 7, ExerciseId = 1, StartDate = new DateTime(2025, 3, 7), EndDate = new DateTime(2025, 5, 1), Repetitions = 10, Sets = 3, Frequency = 2, Notes = "Kniebuiging lichte belasting", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 3, 8, 9, 0, 0) },
                new ExerciseAssignment { Id = 31, PatientId = 7, ExerciseId = 2, StartDate = new DateTime(2025, 3, 7), EndDate = new DateTime(2025, 5, 1), Repetitions = 15, Sets = 3, Frequency = 1, Notes = "Hamstring flexibiliteit knie", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 3, 8, 10, 0, 0) },
                new ExerciseAssignment { Id = 32, PatientId = 7, ExerciseId = 3, StartDate = new DateTime(2025, 3, 20), EndDate = new DateTime(2025, 6, 1), Repetitions = 12, Sets = 3, Frequency = 2, Notes = "Kuitspieren en balans", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 3, 21, 9, 0, 0) },
                new ExerciseAssignment { Id = 33, PatientId = 8, ExerciseId = 4, StartDate = new DateTime(2025, 3, 4), EndDate = new DateTime(2025, 6, 1), Repetitions = 1, Sets = 2, Frequency = 1, Notes = "Algemene core stability", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 4, 16, 9, 0, 0) },
                new ExerciseAssignment { Id = 34, PatientId = 8, ExerciseId = 5, StartDate = new DateTime(2025, 3, 4), EndDate = new DateTime(2025, 6, 1), Repetitions = 10, Sets = 3, Frequency = 1, Notes = "Lichte mobiliteitsoefeningen", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 15, 10, 0, 0) },
                new ExerciseAssignment { Id = 35, PatientId = 9, ExerciseId = 1, StartDate = new DateTime(2025, 3, 12), EndDate = new DateTime(2025, 5, 1), Repetitions = 10, Sets = 3, Frequency = 2, Notes = "Algemene krachtopbouw", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 16, 10, 0, 0) },
                new ExerciseAssignment { Id = 36, PatientId = 9, ExerciseId = 3, StartDate = new DateTime(2025, 3, 12), EndDate = new DateTime(2025, 5, 1), Repetitions = 12, Sets = 3, Frequency = 1, Notes = "Kuit- en enkelstabiliteit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 17, 10, 0, 0) },
                new ExerciseAssignment { Id = 37, PatientId = 10, ExerciseId = 1, StartDate = new DateTime(2025, 4, 7), EndDate = new DateTime(2025, 6, 1), Repetitions = 8, Sets = 3, Frequency = 2, Notes = "Lichte kniebuigingen", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 18, 10, 0, 0) },
                new ExerciseAssignment { Id = 38, PatientId = 10, ExerciseId = 2, StartDate = new DateTime(2025, 4, 7), EndDate = new DateTime(2025, 6, 1), Repetitions = 10, Sets = 3, Frequency = 1, Notes = "Stretch voor mobiliteit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 19, 10, 0, 0) },
                new ExerciseAssignment { Id = 39, PatientId = 11, ExerciseId = 4, StartDate = new DateTime(2025, 4, 10), EndDate = new DateTime(2025, 6, 1), Repetitions = 1, Sets = 3, Frequency = 1, Notes = "Core stability oefeningen", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 20, 10, 0, 0) },
                new ExerciseAssignment { Id = 40, PatientId = 11, ExerciseId = 5, StartDate = new DateTime(2025, 4, 10), EndDate = new DateTime(2025, 6, 1), Repetitions = 10, Sets = 2, Frequency = 1, Notes = "Schouder en nek mobiliteit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 21, 10, 0, 0) },
                new ExerciseAssignment { Id = 41, PatientId = 12, ExerciseId = 1, StartDate = new DateTime(2025, 4, 15), EndDate = new DateTime(2025, 6, 1), Repetitions = 10, Sets = 3, Frequency = 2, Notes = "Lichte belasting oefeningen", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 22, 10, 0, 0) },
                new ExerciseAssignment { Id = 42, PatientId = 12, ExerciseId = 2, StartDate = new DateTime(2025, 4, 15), EndDate = new DateTime(2025, 6, 1), Repetitions = 15, Sets = 3, Frequency = 1, Notes = "Flexibiliteit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 23, 10, 0, 0) },
                new ExerciseAssignment { Id = 43, PatientId = 13, ExerciseId = 1, StartDate = new DateTime(2025, 4, 20), EndDate = new DateTime(2025, 6, 1), Repetitions = 8, Sets = 3, Frequency = 2, Notes = "Knieversterking", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 24, 10, 0, 0) },
                new ExerciseAssignment { Id = 44, PatientId = 13, ExerciseId = 3, StartDate = new DateTime(2025, 4, 20), EndDate = new DateTime(2025, 6, 1), Repetitions = 12, Sets = 3, Frequency = 1, Notes = "Kuitspier activatie", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 25, 10, 0, 0) },
                new ExerciseAssignment { Id = 45, PatientId = 14, ExerciseId = 2, StartDate = new DateTime(2025, 4, 25), EndDate = new DateTime(2025, 6, 1), Repetitions = 10, Sets = 3, Frequency = 2, Notes = "Algemene stretch", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 26, 10, 0, 0) },
                new ExerciseAssignment { Id = 46, PatientId = 14, ExerciseId = 4, StartDate = new DateTime(2025, 4, 25), EndDate = new DateTime(2025, 6, 1), Repetitions = 1, Sets = 3, Frequency = 1, Notes = "Core stability algemeen", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 27, 10, 0, 0) },
                new ExerciseAssignment { Id = 47, PatientId = 15, ExerciseId = 1, StartDate = new DateTime(2025, 5, 1), EndDate = new DateTime(2025, 7, 1), Repetitions = 10, Sets = 3, Frequency = 2, Notes = "Start herstel", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 28, 10, 0, 0) },
                new ExerciseAssignment { Id = 48, PatientId = 15, ExerciseId = 2, StartDate = new DateTime(2025, 5, 1), EndDate = new DateTime(2025, 7, 1), Repetitions = 15, Sets = 3, Frequency = 1, Notes = "Flexibiliteit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 29, 10, 0, 0) },
                new ExerciseAssignment { Id = 49, PatientId = 16, ExerciseId = 3, StartDate = new DateTime(2025, 5, 5), EndDate = new DateTime(2025, 7, 1), Repetitions = 12, Sets = 3, Frequency = 2, Notes = "Kuit en enkel", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 30, 10, 0, 0) },
                new ExerciseAssignment { Id = 50, PatientId = 16, ExerciseId = 4, StartDate = new DateTime(2025, 5, 5), EndDate = new DateTime(2025, 7, 1), Repetitions = 1, Sets = 3, Frequency = 1, Notes = "Core training", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 31, 10, 0, 0) },
                new ExerciseAssignment { Id = 51, PatientId = 17, ExerciseId = 1, StartDate = new DateTime(2025, 5, 10), EndDate = new DateTime(2025, 7, 1), Repetitions = 10, Sets = 3, Frequency = 2, Notes = "Lichte kracht", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 1, 10, 0, 0) },
                new ExerciseAssignment { Id = 52, PatientId = 17, ExerciseId = 5, StartDate = new DateTime(2025, 5, 10), EndDate = new DateTime(2025, 7, 1), Repetitions = 10, Sets = 3, Frequency = 1, Notes = "Schouder mobiliteit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 2, 10, 0, 0) },
                new ExerciseAssignment { Id = 53, PatientId = 18, ExerciseId = 2, StartDate = new DateTime(2025, 5, 15), EndDate = new DateTime(2025, 7, 1), Repetitions = 15, Sets = 3, Frequency = 2, Notes = "Algemene stretch", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 3, 10, 0, 0) },
                new ExerciseAssignment { Id = 54, PatientId = 18, ExerciseId = 3, StartDate = new DateTime(2025, 5, 15), EndDate = new DateTime(2025, 7, 1), Repetitions = 12, Sets = 3, Frequency = 1, Notes = "Kuit en balans", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 4, 10, 0, 0) },
                new ExerciseAssignment { Id = 55, PatientId = 19, ExerciseId = 4, StartDate = new DateTime(2025, 5, 20), EndDate = new DateTime(2025, 7, 1), Repetitions = 1, Sets = 3, Frequency = 1, Notes = "Core stability", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 5, 10, 0, 0) },
                new ExerciseAssignment { Id = 56, PatientId = 19, ExerciseId = 1, StartDate = new DateTime(2025, 5, 20), EndDate = new DateTime(2025, 7, 1), Repetitions = 10, Sets = 3, Frequency = 2, Notes = "Lichte knie", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 6, 10, 0, 0) },
                new ExerciseAssignment { Id = 57, PatientId = 20, ExerciseId = 5, StartDate = new DateTime(2025, 5, 25), EndDate = new DateTime(2025, 7, 1), Repetitions = 10, Sets = 3, Frequency = 1, Notes = "Schouder en nek", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 7, 10, 0, 0) },
                new ExerciseAssignment { Id = 58, PatientId = 20, ExerciseId = 2, StartDate = new DateTime(2025, 5, 25), EndDate = new DateTime(2025, 7, 1), Repetitions = 15, Sets = 3, Frequency = 2, Notes = "Hamstring", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 8, 10, 0, 0) },
                new ExerciseAssignment { Id = 59, PatientId = 21, ExerciseId = 3, StartDate = new DateTime(2025, 5, 28), EndDate = new DateTime(2025, 7, 1), Repetitions = 12, Sets = 3, Frequency = 1, Notes = "Kuitversterking", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 9, 10, 0, 0) },
                new ExerciseAssignment { Id = 60, PatientId = 21, ExerciseId = 1, StartDate = new DateTime(2025, 5, 28), EndDate = new DateTime(2025, 7, 1), Repetitions = 10, Sets = 3, Frequency = 2, Notes = "Kniebuigingen", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 10, 10, 0, 0) },
                new ExerciseAssignment { Id = 61, PatientId = 22, ExerciseId = 4, StartDate = new DateTime(2025, 5, 30), EndDate = new DateTime(2025, 7, 1), Repetitions = 1, Sets = 3, Frequency = 1, Notes = "Core oefeningen", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 11, 10, 0, 0) },
                new ExerciseAssignment { Id = 62, PatientId = 22, ExerciseId = 2, StartDate = new DateTime(2025, 5, 30), EndDate = new DateTime(2025, 7, 1), Repetitions = 10, Sets = 3, Frequency = 2, Notes = "Flexibiliteit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 12, 10, 0, 0) },
                new ExerciseAssignment { Id = 63, PatientId = 23, ExerciseId = 5, StartDate = new DateTime(2025, 6, 1), EndDate = new DateTime(2025, 8, 1), Repetitions = 10, Sets = 3, Frequency = 1, Notes = "Schouder", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 13, 10, 0, 0) },
                new ExerciseAssignment { Id = 64, PatientId = 23, ExerciseId = 1, StartDate = new DateTime(2025, 6, 1), EndDate = new DateTime(2025, 8, 1), Repetitions = 10, Sets = 3, Frequency = 2, Notes = "Knie", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 14, 10, 0, 0) },
                new ExerciseAssignment { Id = 65, PatientId = 24, ExerciseId = 2, StartDate = new DateTime(2025, 6, 5), EndDate = new DateTime(2025, 8, 1), Repetitions = 15, Sets = 3, Frequency = 2, Notes = "Hamstring", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 15, 10, 0, 0) },
                new ExerciseAssignment { Id = 66, PatientId = 24, ExerciseId = 3, StartDate = new DateTime(2025, 6, 5), EndDate = new DateTime(2025, 8, 1), Repetitions = 12, Sets = 3, Frequency = 1, Notes = "Kuit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 16, 10, 0, 0) },
                new ExerciseAssignment { Id = 67, PatientId = 25, ExerciseId = 4, StartDate = new DateTime(2025, 6, 10), EndDate = new DateTime(2025, 8, 1), Repetitions = 1, Sets = 3, Frequency = 1, Notes = "Core", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 17, 10, 0, 0) },
                new ExerciseAssignment { Id = 68, PatientId = 25, ExerciseId = 5, StartDate = new DateTime(2025, 6, 10), EndDate = new DateTime(2025, 8, 1), Repetitions = 10, Sets = 3, Frequency = 3, Notes = "Schouder", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 18, 10, 0, 0) }
            );

            // 5. Pijnindicaties
            modelBuilder.Entity<PainEntry>().HasData(
                new PainEntry { Id = 1, PatientId = 1, Timestamp = new DateTime(2025, 1, 10, 10, 0, 0), Score = 7, Note = "Na de operatie" },
                new PainEntry { Id = 2, PatientId = 1, Timestamp = new DateTime(2025, 1, 11, 09, 0, 0), Score = 6, Note = "lets minder pijn" },
                new PainEntry { Id = 3, PatientId = 1, Timestamp = new DateTime(2025, 1, 12, 09, 30, 0), Score = 5, Note = "Pijn bij bewegen" },
                new PainEntry { Id = 4, PatientId = 1, Timestamp = new DateTime(2025, 1, 13, 09, 0, 0), Score = 5, Note = "Tijdens oefeningen wat meer pijn" },
                new PainEntry { Id = 5, PatientId = 1, Timestamp = new DateTime(2025, 1, 14, 09, 0, 0), Score = 4, Note = "Begin van de dag goed" },
                new PainEntry { Id = 6, PatientId = 1, Timestamp = new DateTime(2025, 1, 15, 09, 0, 0), Score = 4, Note = "Pijn nam licht toe na intensievere oefening" },
                new PainEntry { Id = 7, PatientId = 1, Timestamp = new DateTime(2025, 1, 16, 09, 0, 0), Score = 3, Note = "Gaat steeds beter" },
                new PainEntry { Id = 8, PatientId = 1, Timestamp = new DateTime(2025, 1, 17, 09, 0, 0), Score = 3, Note = "Alleen nog lichte pijn bij langdurig staan" },
                new PainEntry { Id = 9, PatientId = 1, Timestamp = new DateTime(2025, 1, 18, 09, 0, 0), Score = 2, Note = "Bijna pijnvrij in rust" },
                new PainEntry { Id = 10, PatientId = 1, Timestamp = new DateTime(2025, 1, 19, 09, 0, 0), Score = 2, Note = "Soms een steek bij onverwachte beweging" },
                new PainEntry { Id = 11, PatientId = 2, Timestamp = new DateTime(2025, 2, 1, 11, 0, 0), Score = 8, Note = "Erge uitstralende pijn in been" },
                new PainEntry { Id = 12, PatientId = 2, Timestamp = new DateTime(2025, 2, 2, 10, 0, 0), Score = 7, Note = "Medicatie helpt iets" },
                new PainEntry { Id = 13, PatientId = 2, Timestamp = new DateTime(2025, 2, 3, 09, 0, 0), Score = 6, Note = "Na fysiotherapie beter" },
                new PainEntry { Id = 14, PatientId = 2, Timestamp = new DateTime(2025, 2, 4, 09, 0, 0), Score = 6, Note = "Pijn blijft hoog na lange zit" },
                new PainEntry { Id = 15, PatientId = 2, Timestamp = new DateTime(2025, 2, 5, 09, 0, 0), Score = 5, Note = "Iets beter na meer rust" },
                new PainEntry { Id = 16, PatientId = 3, Timestamp = new DateTime(2025, 1, 15, 14, 0, 0), Score = 6, Note = "Zeurende pijn in schouder" },
                new PainEntry { Id = 17, PatientId = 3, Timestamp = new DateTime(2025, 1, 16, 10, 0, 0), Score = 5, Note = "Pijn bij bovenhandse bewegingen" },
                new PainEntry { Id = 18, PatientId = 4, Timestamp = new DateTime(2025, 2, 5, 13, 0, 0), Score = 7, Note = "Flink gezwollen enkel" },
                new PainEntry { Id = 19, PatientId = 4, Timestamp = new DateTime(2025, 2, 6, 09, 0, 0), Score = 6, Note = "Minder zwelling" },
                new PainEntry { Id = 20, PatientId = 5, Timestamp = new DateTime(2025, 3, 1, 16, 0, 0), Score = 9, Note = "Ondraaglijke nekpijn en tintelingen" },
                new PainEntry { Id = 21, PatientId = 5, Timestamp = new DateTime(2025, 3, 2, 09, 0, 0), Score = 8, Note = "Medicatie helpt" }
            );

            // 6. Activiteiten-logboek
            modelBuilder.Entity<ActivityLogEntry>().HasData(
                new ActivityLogEntry { Id = 1, PatientId = 1, Timestamp = new DateTime(2025, 1, 10, 12, 0, 0), Activity = "Rustdag na operatie." },
                new ActivityLogEntry { Id = 2, PatientId = 1, Timestamp = new DateTime(2025, 1, 11, 15, 0, 0), Activity = "Korte wandeling met krukken rondom huis." },
                new ActivityLogEntry { Id = 3, PatientId = 1, Timestamp = new DateTime(2025, 1, 12, 10, 0, 0), Activity = "Huishoudelijke taken (afwas)." },
                new ActivityLogEntry { Id = 4, PatientId = 1, Timestamp = new DateTime(2025, 1, 12, 16, 0, 0), Activity = "Boodschappen gedaan met hulp." },
                new ActivityLogEntry { Id = 5, PatientId = 1, Timestamp = new DateTime(2025, 1, 13, 11, 0, 0), Activity = "30 minuten fysiotherapie oefeningen gedaan." },
                new ActivityLogEntry { Id = 6, PatientId = 1, Timestamp = new DateTime(2025, 1, 13, 17, 0, 0), Activity = "Korte fietstocht op hometrainer (15 min)." },
                new ActivityLogEntry { Id = 7, PatientId = 1, Timestamp = new DateTime(2025, 1, 14, 09, 0, 0), Activity = "Oefeningen volgens schema uitgevoerd." },
                new ActivityLogEntry { Id = 8, PatientId = 1, Timestamp = new DateTime(2025, 1, 14, 14, 0, 0), Activity = "Koken en voorbereiden maaltijden." },
                new ActivityLogEntry { Id = 9, PatientId = 1, Timestamp = new DateTime(2025, 1, 15, 10, 0, 0), Activity = "Oefeningen gedaan" },
                new ActivityLogEntry { Id = 10, PatientId = 1, Timestamp = new DateTime(2025, 1, 15, 16, 0, 0), Activity = "Wandeling buiten" },
                new ActivityLogEntry { Id = 11, PatientId = 2, Timestamp = new DateTime(2025, 2, 1, 14, 0, 0), Activity = "Eerste lichte oefeningen voor rug." },
                new ActivityLogEntry { Id = 12, PatientId = 2, Timestamp = new DateTime(2025, 2, 2, 09, 0, 0), Activity = "Half uur gefietst op hometrainer." },
                new ActivityLogEntry { Id = 13, PatientId = 2, Timestamp = new DateTime(2025, 2, 2, 18, 0, 0), Activity = "Korte wandeling buiten" },
                new ActivityLogEntry { Id = 14, PatientId = 2, Timestamp = new DateTime(2025, 2, 3, 10, 0, 0), Activity = "Rug oefeningen gedaan" },
                new ActivityLogEntry { Id = 15, PatientId = 2, Timestamp = new DateTime(2025, 2, 3, 15, 0, 0), Activity = "Wandeling met hond." },
                new ActivityLogEntry { Id = 16, PatientId = 3, Timestamp = new DateTime(2025, 1, 16, 09, 0, 0), Activity = "Oefeningen voor schouder mobiliteit." },
                new ActivityLogEntry { Id = 17, PatientId = 3, Timestamp = new DateTime(2025, 1, 17, 11, 0, 0), Activity = "Lichte huishoudelijke taken." },
                new ActivityLogEntry { Id = 18, PatientId = 4, Timestamp = new DateTime(2025, 2, 6, 10, 0, 0), Activity = "Enkel oefeningen volgens schema." },
                new ActivityLogEntry { Id = 19, PatientId = 4, Timestamp = new DateTime(2025, 2, 7, 14, 0, 0), Activity = "Korte wandeling binnenshuis." },
                new ActivityLogEntry { Id = 20, PatientId = 5, Timestamp = new DateTime(2025, 3, 3, 10, 0, 0), Activity = "Nekoefeningen met elastiek." },
                new ActivityLogEntry { Id = 21, PatientId = 5, Timestamp = new DateTime(2025, 3, 4, 15, 0, 0), Activity = "Rustig gewandeld." }
            );

            // 7. Medicatie
            modelBuilder.Entity<Medication>().HasData(
                new Medication { Id = 1, PatientId = 1, HuisartsId = 3, Name = "Amoxicilline", Dosage = "500mg", Frequency = "3x daags", StartDate = new DateTime(2025, 1, 8), EndDate = new DateTime(2025, 1, 18), Status = "Afgerond" },
                new Medication { Id = 2, PatientId = 1, HuisartsId = 3, Name = "Paracetamol", Dosage = "500mg", Frequency = "4x daags max", StartDate = new DateTime(2025, 1, 8), EndDate = new DateTime(2025, 2, 1), Status = "Afgerond" },
                new Medication { Id = 3, PatientId = 2, HuisartsId = 4, Name = "Diclofenac", Dosage = "50mg", Frequency = "2x daags", StartDate = new DateTime(2025, 2, 1), EndDate = new DateTime(2025, 3, 1), Status = "Afgerond" },
                new Medication { Id = 4, PatientId = 3, HuisartsId = 3, Name = "Ibuprofen", Dosage = "400mg", Frequency = "3x daags", StartDate = new DateTime(2025, 1, 15), EndDate = new DateTime(2025, 2, 15), Status = "Afgerond" },
                new Medication { Id = 5, PatientId = 4, HuisartsId = 4, Name = "Naproxen", Dosage = "250mg", Frequency = "2x daags", StartDate = new DateTime(2025, 2, 5), EndDate = new DateTime(2025, 3, 5), Status = "Afgerond" },
                new Medication { Id = 6, PatientId = 5, HuisartsId = 3, Name = "Tramadol", Dosage = "50mg", Frequency = "1x daags", StartDate = new DateTime(2025, 2, 3), EndDate = new DateTime(2025, 4, 1), Status = "Actief" },
                new Medication { Id = 7, PatientId = 6, HuisartsId = 4, Name = "Fentanyl pleister", Dosage = "12mcg/u", Frequency = "Elke 72 uur", StartDate = new DateTime(2025, 2, 10), EndDate = new DateTime(2025, 5, 1), Status = "Actief" },
                new Medication { Id = 8, PatientId = 7, HuisartsId = 3, Name = "Pijnstiller X", Dosage = "10mg", Frequency = "3x daags", StartDate = new DateTime(2025, 3, 6), EndDate = new DateTime(2025, 4, 6), Status = "Afgerond" },
                new Medication { Id = 9, PatientId = 8, HuisartsId = 4, Name = "Lyrica", Dosage = "75mg", Frequency = "2x daags", StartDate = new DateTime(2025, 2, 4), EndDate = new DateTime(2025, 6, 2), Status = "Actief" },
                new Medication { Id = 10, PatientId = 9, HuisartsId = 3, Name = "Corticosteroïden", Dosage = "10mg", Frequency = "1x daags", StartDate = new DateTime(2025, 3, 11), EndDate = new DateTime(2025, 3, 25), Status = "Afgerond" },
                new Medication { Id = 11, PatientId = 10, HuisartsId = 4, Name = "Diclofenac gel", Dosage = "Topisch", Frequency = "2x daags", StartDate = new DateTime(2025, 4, 6), EndDate = new DateTime(2025, 5, 6), Status = "Actief" },
                new Medication { Id = 12, PatientId = 11, HuisartsId = 3, Name = "Spierverslapper Y", Dosage = "5mg", Frequency = "1x daags", StartDate = new DateTime(2025, 4, 11), EndDate = new DateTime(2025, 5, 11), Status = "Actief" },
                new Medication { Id = 13, PatientId = 12, HuisartsId = 4, Name = "Pijnstiller Z", Dosage = "20mg", Frequency = "2x daags", StartDate = new DateTime(2025, 4, 16), EndDate = new DateTime(2025, 5, 16), Status = "Actief" },
                new Medication { Id = 14, PatientId = 13, HuisartsId = 3, Name = "Tramadol", Dosage = "50mg", Frequency = "1x daags", StartDate = new DateTime(2025, 4, 21), EndDate = new DateTime(2025, 5, 21), Status = "Actief" },
                new Medication { Id = 15, PatientId = 14, HuisartsId = 4, Name = "Paracetamol", Dosage = "500mg", Frequency = "4x daags", StartDate = new DateTime(2025, 4, 26), EndDate = new DateTime(2025, 5, 26), Status = "Actief" },
                new Medication { Id = 16, PatientId = 15, HuisartsId = 3, Name = "Ibuprofen", Dosage = "400mg", Frequency = "3x daags", StartDate = new DateTime(2025, 5, 2), EndDate = new DateTime(2025, 6, 2), Status = "Actief" },
                new Medication { Id = 17, PatientId = 16, HuisartsId = 4, Name = "Naproxen", Dosage = "250mg", Frequency = "2x daags", StartDate = new DateTime(2025, 5, 6), EndDate = new DateTime(2025, 6, 6), Status = "Actief" },
                new Medication { Id = 18, PatientId = 17, HuisartsId = 3, Name = "Codeine", Dosage = "30mg", Frequency = "2x daags", StartDate = new DateTime(2025, 5, 11), EndDate = new DateTime(2025, 6, 11), Status = "Actief" },
                new Medication { Id = 19, PatientId = 18, HuisartsId = 4, Name = "Paracetamol", Dosage = "500mg", Frequency = "4x daags", StartDate = new DateTime(2025, 5, 16), EndDate = new DateTime(2025, 6, 16), Status = "Actief" },
                new Medication { Id = 20, PatientId = 19, HuisartsId = 3, Name = "Diclofenac", Dosage = "50mg", Frequency = "2x daags", StartDate = new DateTime(2025, 5, 21), EndDate = new DateTime(2025, 6, 21), Status = "Actief" },
                new Medication { Id = 21, PatientId = 20, HuisartsId = 4, Name = "Ibuprofen", Dosage = "400mg", Frequency = "3x daags", StartDate = new DateTime(2025, 5, 26), EndDate = new DateTime(2025, 6, 26), Status = "Actief" },
                new Medication { Id = 22, PatientId = 21, HuisartsId = 3, Name = "Paracetamol", Dosage = "500mg", Frequency = "4x daags", StartDate = new DateTime(2025, 5, 29), EndDate = new DateTime(2025, 6, 29), Status = "Actief" },
                new Medication { Id = 23, PatientId = 22, HuisartsId = 4, Name = "Naproxen", Dosage = "250mg", Frequency = "2x daags", StartDate = new DateTime(2025, 5, 31), EndDate = new DateTime(2025, 6, 30), Status = "Actief" },
                new Medication { Id = 24, PatientId = 23, HuisartsId = 3, Name = "Tramadol", Dosage = "50mg", Frequency = "1x daags", StartDate = new DateTime(2025, 6, 2), EndDate = new DateTime(2025, 7, 2), Status = "Actief" },
                new Medication { Id = 25, PatientId = 24, HuisartsId = 4, Name = "Pijnstiller A", Dosage = "10mg", Frequency = "3x daags", StartDate = new DateTime(2025, 6, 6), EndDate = new DateTime(2025, 7, 6), Status = "Actief" }
            );

            // 8. Accessoires
            modelBuilder.Entity<AccessoryAdvice>().HasData(
                new AccessoryAdvice { Id = 1, PatientId = 1, HuisartsId = 3, Name = "Krukken", AdviceDate = new DateTime(2025, 1, 9), ExpectedUsagePeriod = "6 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 2, PatientId = 1, HuisartsId = 3, Name = "Kniebrace", AdviceDate = new DateTime(2025, 1, 15), ExpectedUsagePeriod = "12 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 3, PatientId = 2, HuisartsId = 4, Name = "Rugbrace", AdviceDate = new DateTime(2025, 2, 1), ExpectedUsagePeriod = "8 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 4, PatientId = 3, HuisartsId = 3, Name = "Schouderbrace", AdviceDate = new DateTime(2025, 1, 16), ExpectedUsagePeriod = "4 weken", Status = "Afgerond" },
                new AccessoryAdvice { Id = 5, PatientId = 4, HuisartsId = 4, Name = "Enkelbrace", AdviceDate = new DateTime(2025, 2, 6), ExpectedUsagePeriod = "6 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 6, PatientId = 5, HuisartsId = 3, Name = "Nekbrace", AdviceDate = new DateTime(2025, 3, 3), ExpectedUsagePeriod = "8 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 7, PatientId = 6, HuisartsId = 4, Name = "Fysiotherapie banden", AdviceDate = new DateTime(2025, 2, 12), ExpectedUsagePeriod = "Onbepaald", Status = "Actief" },
                new AccessoryAdvice { Id = 8, PatientId = 7, HuisartsId = 3, Name = "Meniscusbrace", AdviceDate = new DateTime(2025, 3, 7), ExpectedUsagePeriod = "10 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 9, PatientId = 8, HuisartsId = 4, Name = "TENS apparaat", AdviceDate = new DateTime(2025, 3, 4), ExpectedUsagePeriod = "Onbepaald", Status = "Actief" },
                new AccessoryAdvice { Id = 10, PatientId = 9, HuisartsId = 3, Name = "Polsbrace", AdviceDate = new DateTime(2025, 3, 12), ExpectedUsagePeriod = "4 weken", Status = "Afgerond" },
                new AccessoryAdvice { Id = 11, PatientId = 10, HuisartsId = 4, Name = "Kniebrace", AdviceDate = new DateTime(2025, 4, 7), ExpectedUsagePeriod = "8 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 12, PatientId = 11, HuisartsId = 3, Name = "Rugkussen", AdviceDate = new DateTime(2025, 4, 11), ExpectedUsagePeriod = "Onbepaald", Status = "Actief" },
                new AccessoryAdvice { Id = 13, PatientId = 12, HuisartsId = 4, Name = "Enkelgewichtjes", AdviceDate = new DateTime(2025, 4, 16), ExpectedUsagePeriod = "6 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 14, PatientId = 13, HuisartsId = 3, Name = "Looprek", AdviceDate = new DateTime(2025, 4, 21), ExpectedUsagePeriod = "12 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 15, PatientId = 14, HuisartsId = 4, Name = "Schouderkatrol", AdviceDate = new DateTime(2025, 4, 26), ExpectedUsagePeriod = "8 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 16, PatientId = 15, HuisartsId = 3, Name = "Knieband", AdviceDate = new DateTime(2025, 5, 2), ExpectedUsagePeriod = "6 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 17, PatientId = 16, HuisartsId = 4, Name = "Enkelfles", AdviceDate = new DateTime(2025, 5, 6), ExpectedUsagePeriod = "Onbepaald", Status = "Actief" },
                new AccessoryAdvice { Id = 18, PatientId = 17, HuisartsId = 3, Name = "Krukken", AdviceDate = new DateTime(2025, 5, 11), ExpectedUsagePeriod = "4 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 19, PatientId = 18, HuisartsId = 4, Name = "Rugbrace", AdviceDate = new DateTime(2025, 5, 16), ExpectedUsagePeriod = "8 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 20, PatientId = 19, HuisartsId = 3, Name = "Schouderriem", AdviceDate = new DateTime(2025, 5, 21), ExpectedUsagePeriod = "6 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 21, PatientId = 20, HuisartsId = 4, Name = "Enkelondersteuning", AdviceDate = new DateTime(2025, 5, 26), ExpectedUsagePeriod = "4 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 22, PatientId = 21, HuisartsId = 3, Name = "Kniebrace", AdviceDate = new DateTime(2025, 5, 29), ExpectedUsagePeriod = "10 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 23, PatientId = 22, HuisartsId = 4, Name = "Hielspoorzooltjes", AdviceDate = new DateTime(2025, 5, 31), ExpectedUsagePeriod = "Onbepaald", Status = "Actief" },
                new AccessoryAdvice { Id = 24, PatientId = 23, HuisartsId = 3, Name = "Rolstoel", AdviceDate = new DateTime(2025, 6, 2), ExpectedUsagePeriod = "12 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 25, PatientId = 24, HuisartsId = 4, Name = "Handtrainer", AdviceDate = new DateTime(2025, 6, 6), ExpectedUsagePeriod = "Onbepaald", Status = "Actief" }
            );

            // 9. Afspraken
            modelBuilder.Entity<Appointment>().HasData(
                new Appointment { Id = 1, PatientId = 1, DoctorId = 5, DateTime = new DateTime(2025, 1, 10, 14, 0, 0), Duration = 60, Type = "Intake", Status = "Afgerond" },
                new Appointment { Id = 2, PatientId = 1, DoctorId = 5, DateTime = new DateTime(2025, 1, 31, 10, 0, 0), Duration = 45, Type = "Controle", Status = "Afgerond" },
                new Appointment { Id = 3, PatientId = 1, DoctorId = 5, DateTime = new DateTime(2025, 2, 21, 11, 0, 0), Duration = 45, Type = "Controle", Status = "Afgerond" },
                new Appointment { Id = 4, PatientId = 1, DoctorId = 5, DateTime = new DateTime(2025, 3, 14, 13, 0, 0), Duration = 30, Type = "Controle", Status = "Afgerond" },
                new Appointment { Id = 5, PatientId = 1, DoctorId = 5, DateTime = new DateTime(2025, 4, 4, 09, 0, 0), Duration = 30, Type = "Controle", Status = "Gepland" },
                new Appointment { Id = 6, PatientId = 1, DoctorId = 5, DateTime = new DateTime(2025, 4, 25, 10, 0, 0), Duration = 30, Type = "Controle", Status = "Gepland" },
                new Appointment { Id = 7, PatientId = 2, DoctorId = 6, DateTime = new DateTime(2025, 2, 1, 09, 0, 0), Duration = 60, Type = "Intake", Status = "Afgerond" },
                new Appointment { Id = 8, PatientId = 2, DoctorId = 6, DateTime = new DateTime(2025, 2, 22, 14, 0, 0), Duration = 45, Type = "Controle", Status = "Afgerond" },
                new Appointment { Id = 9, PatientId = 2, DoctorId = 6, DateTime = new DateTime(2025, 3, 15, 10, 0, 0), Duration = 45, Type = "Gepland", Status = "Gepland" },
                new Appointment { Id = 10, PatientId = 2, DoctorId = 6, DateTime = new DateTime(2025, 4, 5, 11, 0, 0), Duration = 30, Type = "Controle", Status = "Gepland" },
                new Appointment { Id = 11, PatientId = 3, DoctorId = 5, DateTime = new DateTime(2025, 1, 15, 16, 0, 0), Duration = 60, Type = "Intake", Status = "Afgerond" },
                new Appointment { Id = 12, PatientId = 3, DoctorId = 5, DateTime = new DateTime(2025, 2, 5, 15, 0, 0), Duration = 45, Type = "Controle", Status = "Afgerond" },
                new Appointment { Id = 13, PatientId = 3, DoctorId = 5, DateTime = new DateTime(2025, 2, 26, 14, 0, 0), Duration = 30, Type = "Controle", Status = "Gepland" },
                new Appointment { Id = 14, PatientId = 4, DoctorId = 7, DateTime = new DateTime(2025, 2, 5, 10, 0, 0), Duration = 60, Type = "Intake", Status = "Afgerond" },
                new Appointment { Id = 15, PatientId = 4, DoctorId = 7, DateTime = new DateTime(2025, 2, 26, 11, 0, 0), Duration = 45, Type = "Controle", Status = "Afgerond" },
                new Appointment { Id = 16, PatientId = 4, DoctorId = 7, DateTime = new DateTime(2025, 3, 19, 13, 0, 0), Duration = 30, Type = "Controle", Status = "Gepland" },
                new Appointment { Id = 17, PatientId = 5, DoctorId = 6, DateTime = new DateTime(2025, 3, 1, 09, 0, 0), Duration = 60, Type = "Intake", Status = "Afgerond" },
                new Appointment { Id = 18, PatientId = 5, DoctorId = 6, DateTime = new DateTime(2025, 3, 22, 10, 0, 0), Duration = 45, Type = "Controle", Status = "Gepland" },
                new Appointment { Id = 19, PatientId = 6, DoctorId = 5, DateTime = new DateTime(2025, 2, 10, 14, 0, 0), Duration = 60, Type = "Intake", Status = "Afgerond" },
                new Appointment { Id = 20, PatientId = 6, DoctorId = 5, DateTime = new DateTime(2025, 3, 3, 15, 0, 0), Duration = 45, Type = "Controle", Status = "Gepland" }
            );

            // 10. Intakegesprekken
            modelBuilder.Entity<IntakeRecord>().HasData(
                new IntakeRecord { Id = 1, PatientId = 1, DoctorId = 5, Diagnosis = "Gescheurde kruisbanden (ACL)", Severity = "Ernstig", InitialGoals = "Volledig herstel kniefunctie; wondgenezing; pijnmanagement", Date = new DateTime(2025, 1, 10) },
                new IntakeRecord { Id = 2, PatientId = 2, DoctorId = 6, Diagnosis = "Hernia L5-S1", Severity = "Matig", InitialGoals = "Pijnreductie; mobiliteit; terugkeer naar werk", Date = new DateTime(2025, 2, 1) },
                new IntakeRecord { Id = 3, PatientId = 3, DoctorId = 5, Diagnosis = "Schouder impingement", Severity = "Licht tot matig", InitialGoals = "Pijnvrij bewegen; krachtopbouw", Date = new DateTime(2025, 1, 15) },
                new IntakeRecord { Id = 4, PatientId = 4, DoctorId = 7, Diagnosis = "Enkelverstuiking graad 2", Severity = "Matig", InitialGoals = "Stabiliteit; belasting opbouw", Date = new DateTime(2025, 2, 5) },
                new IntakeRecord { Id = 5, PatientId = 5, DoctorId = 6, Diagnosis = "Nekhernia C5-C6", Severity = "Ernstig", InitialGoals = "Nekmobiliteit; zenuwherstel; pijncontrole", Date = new DateTime(2025, 3, 1) },
                new IntakeRecord { Id = 6, PatientId = 6, DoctorId = 5, Diagnosis = "Chronische rugpijn", Severity = "Matig", InitialGoals = "Activatie; coping mechanismen; functiebehoud", Date = new DateTime(2025, 2, 10) },
                new IntakeRecord { Id = 7, PatientId = 7, DoctorId = 7, Diagnosis = "Gescheurde meniscus", Severity = "Ernstig", InitialGoals = "Volledig herstel kniefunctie; pijnvrij sporten", Date = new DateTime(2025, 3, 5) },
                new IntakeRecord { Id = 8, PatientId = 8, DoctorId = 6, Diagnosis = "Fibromyalgie", Severity = "Chronisch", InitialGoals = "Pijnmanagement; energieniveau; levenskwaliteit", Date = new DateTime(2025, 4, 1) },
                new IntakeRecord { Id = 9, PatientId = 9, DoctorId = 5, Diagnosis = "Carpale tunnelsyndroom", Severity = "Licht", InitialGoals = "Pijnreductie; grijpkracht herstel", Date = new DateTime(2025, 3, 10) },
                new IntakeRecord { Id = 10, PatientId = 10, DoctorId = 7, Diagnosis = "Knieartrose", Severity = "Matig", InitialGoals = "Mobiliteit; pijnvermindering; spierversterking", Date = new DateTime(2025, 4, 5) }
            );

            // 11. Notities
            modelBuilder.Entity<PatientNote>().HasData(
                new PatientNote { Id = 1, PatientId = 1, AuthorUserId = 3, Timestamp = new DateTime(2025, 1, 9, 16, 0, 0), Content = "Wondinfectie arm lijkt te verbeteren na start antibiotica." },
                new PatientNote { Id = 2, PatientId = 1, AuthorUserId = 5, Timestamp = new DateTime(2025, 1, 10, 15, 0, 0), Content = "Patiënt is gemotiveerd" },
                new PatientNote { Id = 3, PatientId = 1, AuthorUserId = 3, Timestamp = new DateTime(2025, 1, 20, 10, 0, 0), Content = "Wondgenezing goed" },
                new PatientNote { Id = 4, PatientId = 1, AuthorUserId = 5, Timestamp = new DateTime(2025, 1, 31, 10, 45, 0), Content = "Voortgang positief" },
                new PatientNote { Id = 5, PatientId = 2, AuthorUserId = 4, Timestamp = new DateTime(2025, 2, 5, 09, 30, 0), Content = "Patiënt klaagt over zenuwpijn" },
                new PatientNote { Id = 6, PatientId = 2, AuthorUserId = 6, Timestamp = new DateTime(2025, 2, 6, 11, 0, 0), Content = "Zenuwpijn besproken" },
                new PatientNote { Id = 7, PatientId = 3, AuthorUserId = 3, Timestamp = new DateTime(2025, 1, 17, 14, 30, 0), Content = "Schouderklachten lijken te stabiliseren." },
                new PatientNote { Id = 8, PatientId = 3, AuthorUserId = 5, Timestamp = new DateTime(2025, 1, 17, 17, 0, 0), Content = "Oefenplan opgesteld voor schouder mobiliteit." },
                new PatientNote { Id = 9, PatientId = 4, AuthorUserId = 4, Timestamp = new DateTime(2025, 2, 7, 10, 30, 0), Content = "Enkel nog steeds gezwollen" },
                new PatientNote { Id = 10, PatientId = 4, AuthorUserId = 7, Timestamp = new DateTime(2025, 2, 7, 11, 30, 0), Content = "Start oefenplan voor enkelstabiliteit." },
                new PatientNote { Id = 11, PatientId = 5, AuthorUserId = 3, Timestamp = new DateTime(2025, 3, 3, 10, 0, 0), Content = "Nekpijn nog ernstig" },
                new PatientNote { Id = 12, PatientId = 5, AuthorUserId = 6, Timestamp = new DateTime(2025, 3, 3, 10, 45, 0), Content = "Intake afgerond" },
                new PatientNote { Id = 13, PatientId = 6, AuthorUserId = 4, Timestamp = new DateTime(2025, 2, 12, 14, 0, 0), Content = "Chronische rugpijn besproken" },
                new PatientNote { Id = 14, PatientId = 6, AuthorUserId = 5, Timestamp = new DateTime(2025, 2, 12, 15, 0, 0), Content = "Activatieprogramma gestart voor rugpijn." },
                new PatientNote { Id = 15, PatientId = 7, AuthorUserId = 3, Timestamp = new DateTime(2025, 3, 7, 10, 0, 0), Content = "Meniscus scheur" },
                new PatientNote { Id = 16, PatientId = 7, AuthorUserId = 7, Timestamp = new DateTime(2025, 3, 7, 11, 0, 0), Content = "Focus op herstel kniefunctie voor sportterugkeer." },
                new PatientNote { Id = 17, PatientId = 8, AuthorUserId = 4, Timestamp = new DateTime(2025, 4, 3, 10, 0, 0), Content = "Fibromyalgie patiënt" },
                new PatientNote { Id = 18, PatientId = 8, AuthorUserId = 6, Timestamp = new DateTime(2025, 4, 3, 11, 0, 0), Content = "Behandelplan gericht op energiebeheer en pijnreductie." },
                new PatientNote { Id = 19, PatientId = 9, AuthorUserId = 3, Timestamp = new DateTime(2025, 3, 12, 10, 0, 0), Content = "CT-scan uitslag besproken" },
                new PatientNote { Id = 20, PatientId = 9, AuthorUserId = 5, Timestamp = new DateTime(2025, 3, 12, 11, 0, 0), Content = "Start fysiotherapie voor pols en hand." }
            );

            // 12. Declaraties Revalidatiearts
            modelBuilder.Entity<Declaration>().HasData(
                new Declaration { Id = 27, PatientId = 1, DoctorId = 5, Date = new DateTime(2025, 1, 10), TreatmentType = "Intake revalidatie knie", Amount = 8000m, Status = "Geregistreerd" },
                new Declaration { Id = 28, PatientId = 1, DoctorId = 5, Date = new DateTime(2025, 1, 31), TreatmentType = "Controle consult 1", Amount = 6000m, Status = "Geregistreerd" },
                new Declaration { Id = 29, PatientId = 1, DoctorId = 5, Date = new DateTime(2025, 2, 21), TreatmentType = "Controle consult 2", Amount = 6000m, Status = "Geregistreerd" },
                new Declaration { Id = 30, PatientId = 1, DoctorId = 5, Date = new DateTime(2025, 3, 14), TreatmentType = "Controle consult 3", Amount = 6000m, Status = "Geregistreerd" },
                new Declaration { Id = 31, PatientId = 1, DoctorId = 5, Date = new DateTime(2025, 4, 4), TreatmentType = "Controle consult 4", Amount = 6000m, Status = "Geregistreerd" },
                new Declaration { Id = 32, PatientId = 2, DoctorId = 6, Date = new DateTime(2025, 1, 2), TreatmentType = "Intake revalidatie rug", Amount = 8000m, Status = "Geregistreerd" },
                new Declaration { Id = 33, PatientId = 2, DoctorId = 6, Date = new DateTime(2025, 2, 22), TreatmentType = "Controle consult 1", Amount = 6000m, Status = "Geregistreerd" },
                new Declaration { Id = 34, PatientId = 2, DoctorId = 6, Date = new DateTime(2025, 3, 15), TreatmentType = "Controle consult 2", Amount = 6000m, Status = "Geregistreerd" },
                new Declaration { Id = 35, PatientId = 3, DoctorId = 5, Date = new DateTime(2025, 1, 15), TreatmentType = "Intake revalidatie schouder", Amount = 8000m, Status = "Geregistreerd" },
                new Declaration { Id = 36, PatientId = 3, DoctorId = 5, Date = new DateTime(2025, 2, 5), TreatmentType = "Controle consult 1", Amount = 6000m, Status = "Geregistreerd" },
                new Declaration { Id = 37, PatientId = 4, DoctorId = 7, Date = new DateTime(2025, 2, 5), TreatmentType = "Intake revalidatie enkel", Amount = 8000m, Status = "Geregistreerd" },
                new Declaration { Id = 38, PatientId = 4, DoctorId = 7, Date = new DateTime(2025, 2, 26), TreatmentType = "Controle consult 1", Amount = 6000m, Status = "Geregistreerd" },
                new Declaration { Id = 39, PatientId = 5, DoctorId = 6, Date = new DateTime(2025, 3, 1), TreatmentType = "Intake revalidatie nek", Amount = 8000m, Status = "Geregistreerd" },
                new Declaration { Id = 40, PatientId = 5, DoctorId = 6, Date = new DateTime(2025, 3, 22), TreatmentType = "Controle consult 1", Amount = 6000m, Status = "Geregistreerd" },
                new Declaration { Id = 41, PatientId = 6, DoctorId = 5, Date = new DateTime(2025, 2, 10), TreatmentType = "Intake chronische rugpijn", Amount = 8000m, Status = "Geregistreerd" },
                new Declaration { Id = 42, PatientId = 6, DoctorId = 5, Date = new DateTime(2025, 3, 3), TreatmentType = "Controle consult 1", Amount = 6000m, Status = "Geregistreerd" },
                new Declaration { Id = 43, PatientId = 7, DoctorId = 7, Date = new DateTime(2025, 3, 5), TreatmentType = "Intake meniscus", Amount = 8000m, Status = "Geregistreerd" },
                new Declaration { Id = 44, PatientId = 7, DoctorId = 7, Date = new DateTime(2025, 3, 26), TreatmentType = "Controle consult 1", Amount = 6000m, Status = "Geregistreerd" },
                new Declaration { Id = 45, PatientId = 8, DoctorId = 6, Date = new DateTime(2025, 4, 1), TreatmentType = "Intake fibromyalgie", Amount = 8000m, Status = "Geregistreerd" },
                new Declaration { Id = 46, PatientId = 8, DoctorId = 6, Date = new DateTime(2025, 4, 22), TreatmentType = "Controle consult 1", Amount = 6000m, Status = "Geregistreerd" },
                new Declaration { Id = 47, PatientId = 9, DoctorId = 5, Date = new DateTime(2025, 3, 10), TreatmentType = "Intake CTS", Amount = 8000m, Status = "Geregistreerd" },
                new Declaration { Id = 48, PatientId = 9, DoctorId = 5, Date = new DateTime(2025, 3, 31), TreatmentType = "Controle consult 1", Amount = 6000m, Status = "Geregistreerd" },
                new Declaration { Id = 49, PatientId = 10, DoctorId = 7, Date = new DateTime(2025, 4, 5), TreatmentType = "Intake knie artrose", Amount = 8000m, Status = "Geregistreerd" },
                new Declaration { Id = 50, PatientId = 10, DoctorId = 7, Date = new DateTime(2025, 4, 26), TreatmentType = "Controle consult 1", Amount = 6000m, Status = "Geregistreerd" },
                new Declaration { Id = 51, PatientId = 11, DoctorId = 6, Date = new DateTime(2025, 4, 10), TreatmentType = "Intake algemeen", Amount = 8000m, Status = "Geregistreerd" },
                new Declaration { Id = 52, PatientId = 11, DoctorId = 6, Date = new DateTime(2025, 5, 1), TreatmentType = "Controle consult 1", Amount = 6000m, Status = "Geregistreerd" },
                new Declaration { Id = 53, PatientId = 12, DoctorId = 5, Date = new DateTime(2025, 4, 15), TreatmentType = "Intake revalidatie", Amount = 8000m, Status = "Geregistreerd" },
                new Declaration { Id = 54, PatientId = 12, DoctorId = 5, Date = new DateTime(2025, 5, 6), TreatmentType = "Controle consult 1", Amount = 6000m, Status = "Geregistreerd" },
                new Declaration { Id = 55, PatientId = 13, DoctorId = 7, Date = new DateTime(2025, 4, 20), TreatmentType = "Intake oudere patiënt", Amount = 8000m, Status = "Geregistreerd" },
                new Declaration { Id = 56, PatientId = 13, DoctorId = 7, Date = new DateTime(2025, 5, 11), TreatmentType = "Controle consult 1", Amount = 6000m, Status = "Geregistreerd" },
                new Declaration { Id = 57, PatientId = 14, DoctorId = 6, Date = new DateTime(2025, 4, 25), TreatmentType = "Intake diverse klachten", Amount = 8000m, Status = "Geregistreerd" },
                new Declaration { Id = 58, PatientId = 14, DoctorId = 6, Date = new DateTime(2025, 5, 16), TreatmentType = "Controle consult 1", Amount = 6000m, Status = "Geregistreerd" },
                new Declaration { Id = 59, PatientId = 15, DoctorId = 5, Date = new DateTime(2025, 5, 1), TreatmentType = "Intake nieuwe patiënt", Amount = 8000m, Status = "Geregistreerd" },
                new Declaration { Id = 60, PatientId = 15, DoctorId = 5, Date = new DateTime(2025, 5, 22), TreatmentType = "Controle consult 1", Amount = 6000m, Status = "Geregistreerd" },
                new Declaration { Id = 61, PatientId = 16, DoctorId = 7, Date = new DateTime(2025, 5, 5), TreatmentType = "Intake chronische pijn", Amount = 8000m, Status = "Geregistreerd" },
                new Declaration { Id = 62, PatientId = 16, DoctorId = 7, Date = new DateTime(2025, 5, 26), TreatmentType = "Controle consult 1", Amount = 6000m, Status = "Geregistreerd" },
                new Declaration { Id = 63, PatientId = 17, DoctorId = 6, Date = new DateTime(2025, 5, 10), TreatmentType = "Intake post-op", Amount = 8000m, Status = "Geregistreerd" },
                new Declaration { Id = 64, PatientId = 17, DoctorId = 6, Date = new DateTime(2025, 5, 31), TreatmentType = "Controle consult 1", Amount = 6000m, Status = "Geregistreerd" },
                new Declaration { Id = 65, PatientId = 18, DoctorId = 5, Date = new DateTime(2025, 5, 15), TreatmentType = "Intake complexe case", Amount = 8000m, Status = "Geregistreerd" },
                new Declaration { Id = 66, PatientId = 18, DoctorId = 5, Date = new DateTime(2025, 6, 5), TreatmentType = "Controle consult 1", Amount = 6000m, Status = "Geregistreerd" },
                new Declaration { Id = 67, PatientId = 19, DoctorId = 7, Date = new DateTime(2025, 5, 20), TreatmentType = "Intake sportblessure", Amount = 8000m, Status = "Geregistreerd" },
                new Declaration { Id = 68, PatientId = 19, DoctorId = 7, Date = new DateTime(2025, 6, 10), TreatmentType = "Controle consult 1", Amount = 6000m, Status = "Geregistreerd" },
                new Declaration { Id = 69, PatientId = 20, DoctorId = 6, Date = new DateTime(2025, 5, 25), TreatmentType = "Intake revalidatieplan", Amount = 8000m, Status = "Geregistreerd" },
                new Declaration { Id = 70, PatientId = 20, DoctorId = 6, Date = new DateTime(2025, 6, 15), TreatmentType = "Controle consult 1", Amount = 6000m, Status = "Geregistreerd" },
                new Declaration { Id = 71, PatientId = 21, DoctorId = 5, Date = new DateTime(2025, 5, 28), TreatmentType = "Intake algemeen herstel", Amount = 8000m, Status = "Geregistreerd" },
                new Declaration { Id = 72, PatientId = 21, DoctorId = 5, Date = new DateTime(2025, 6, 18), TreatmentType = "Controle consult 1", Amount = 6000m, Status = "Geregistreerd" },
                new Declaration { Id = 73, PatientId = 22, DoctorId = 7, Date = new DateTime(2025, 5, 30), TreatmentType = "Intake chronische aandoening", Amount = 8000m, Status = "Geregistreerd" },
                new Declaration { Id = 74, PatientId = 22, DoctorId = 7, Date = new DateTime(2025, 6, 20), TreatmentType = "Controle consult 1", Amount = 6000m, Status = "Geregistreerd" },
                new Declaration { Id = 75, PatientId = 23, DoctorId = 6, Date = new DateTime(2025, 6, 1), TreatmentType = "Intake nieuwe doorverwijzing", Amount = 8000m, Status = "Geregistreerd" },
                new Declaration { Id = 76, PatientId = 23, DoctorId = 6, Date = new DateTime(2025, 6, 22), TreatmentType = "Controle consult 1", Amount = 6000m, Status = "Geregistreerd" },
                new Declaration { Id = 77, PatientId = 24, DoctorId = 5, Date = new DateTime(2025, 6, 5), TreatmentType = "Intake breukherstel", Amount = 8000m, Status = "Geregistreerd" },
                new Declaration { Id = 78, PatientId = 24, DoctorId = 5, Date = new DateTime(2025, 6, 26), TreatmentType = "Controle consult 1", Amount = 6000m, Status = "Geregistreerd" },
                new Declaration { Id = 79, PatientId = 25, DoctorId = 7, Date = new DateTime(2025, 6, 10), TreatmentType = "Intake algemeen traject", Amount = 8000m, Status = "Geregistreerd" },
                new Declaration { Id = 80, PatientId = 25, DoctorId = 7, Date = new DateTime(2025, 7, 1), TreatmentType = "Controle consult 1", Amount = 6000m, Status = "Geregistreerd" }
            );
        }
    }
}