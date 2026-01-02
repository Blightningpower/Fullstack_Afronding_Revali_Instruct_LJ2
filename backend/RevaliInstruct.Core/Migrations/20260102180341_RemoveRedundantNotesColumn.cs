using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RevaliInstruct.Core.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRedundantNotesColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VideoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganisatieId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AssignedDoctorUserId = table.Column<int>(type: "int", nullable: false),
                    ReferringDoctorUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Patients_Users_AssignedDoctorUserId",
                        column: x => x.AssignedDoctorUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Patients_Users_ReferringDoctorUserId",
                        column: x => x.ReferringDoctorUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccessoryAdvices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    HuisartsId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdviceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpectedUsagePeriod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessoryAdvices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessoryAdvices_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActivityLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activity = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityLogs_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    AppointmentDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ExerciseAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    ExerciseId = table.Column<int>(type: "int", nullable: false),
                    Repetitions = table.Column<int>(type: "int", nullable: false),
                    Sets = table.Column<int>(type: "int", nullable: false),
                    Frequency = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientCheckedOff = table.Column<bool>(type: "bit", nullable: false),
                    CheckedOffAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseAssignments_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExerciseAssignments_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IntakeRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    Diagnosis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Goals = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntakeRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IntakeRecords_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Medication",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    HuisartsId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dosage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Frequency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Medication_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PainEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PainEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PainEntries_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    AuthorUserId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientNotes_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Exercises",
                columns: new[] { "Id", "Description", "Name", "VideoUrl" },
                values: new object[,]
                {
                    { 1, "Langzaam knieën buigen tot 90 graden rug recht houden.", "Kniebuiging", "https://www.youtube.com/watch?v=00g8DINXtIY" },
                    { 2, "Zittend één been gestrekt", "Hamstring Stretch", "https://www.youtube.com/shorts/ywgDXzYD6v8" },
                    { 3, "Op de tenen staan en langzaam zakken.", "Calf Raises", "https://www.youtube.com/watch?v=eMTy3qylqnE" },
                    { 4, "Plankpositie rug recht", "Core Stability Plank", "https://www.youtube.com/watch?v=pvIjsG5Svck" },
                    { 5, "Cirkelbewegingen met de armen.", "Schouder Rotaties", "https://www.youtube.com/watch?v=oLwTC-lAJws" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "FirstName", "LastName", "OrganisatieId", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 1, "Sophie", "Jansen", null, "$2y$10$9mIxp6HbC1KobigZYb0qUu98xe11w45XH1kvPKX2qZ44BsF2qObDy", "Zorgverzekeraar", "zvm_jansen" },
                    { 2, "Mark", "de Vries", null, "$2y$10$9mIxp6HbC1KobigZYb0qUu98xe11w45XH1kvPKX2qZ44BsF2qObDy", "Zorgverzekeraar", "zvm_devries" },
                    { 3, "Lisa", "Bakker", null, "$2y$10$9mIxp6HbC1KobigZYb0qUu98xe11w45XH1kvPKX2qZ44BsF2qObDy", "Huisarts", "ha_bakker" },
                    { 4, "Thomas", "Janssen", null, "$2y$10$9mIxp6HbC1KobigZYb0qUu98xe11w45XH1kvPKX2qZ44BsF2qObDy", "Huisarts", "ha_janssen" },
                    { 5, "Emma", "Smit", null, "$2y$10$9mIxp6HbC1KobigZYb0qUu98xe11w45XH1kvPKX2qZ44BsF2qObDy", "Revalidatiearts", "ra_smit" },
                    { 6, "David", "Groen", null, "$2y$10$9mIxp6HbC1KobigZYb0qUu98xe11w45XH1kvPKX2qZ44BsF2qObDy", "Revalidatiearts", "ra_groen" },
                    { 7, "Linda", "Visser", null, "$2y$10$9mIxp6HbC1KobigZYb0qUu98xe11w45XH1kvPKX2qZ44BsF2qObDy", "Revalidatiearts", "ra_visser" }
                });

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "Id", "Address", "AssignedDoctorUserId", "BirthDate", "Email", "FirstName", "LastName", "Phone", "ReferringDoctorUserId", "StartDate", "Status" },
                values: new object[,]
                {
                    { 1, "Kruisbandstraat 10", 5, new DateTime(1995, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "freddy.v@example.com", "Freddy", "Voetballer", "0612345678", 3, new DateTime(2025, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, "Therapiestraat 5", 6, new DateTime(1988, 11, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "anna.k@example.com", "Anna", "Kamer", "0623456789", 4, new DateTime(2025, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 3, "Oefenlaan 12", 5, new DateTime(1976, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "bas.v@example.com", "Bas", "Verkade", "0634567890", 3, new DateTime(2025, 1, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 4, "Herstelweg 3", 7, new DateTime(2001, 1, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "carla.d@example.com", "Carla", "Dirksen", "0645678901", 4, new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 5, "Revalidatieplein 7", 6, new DateTime(1965, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "dirk.m@example.com", "Dirk", "Meijer", "0656789012", 3, new DateTime(2025, 2, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 6, "Fysioweg 22", 5, new DateTime(1999, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "eva.p@example.com", "Eva", "Peters", "0667890123", 4, new DateTime(2025, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 7, "Gipsstraat 1", 7, new DateTime(1970, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "frank.dj@example.com", "Frank", "de Jong", "0678901234", 3, new DateTime(2025, 3, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 8, "Rolstoelpad 8", 6, new DateTime(1985, 12, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "greetje.w@example.com", "Greetje", "Willems", "0689012345", 4, new DateTime(2025, 3, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 9, "Sportlaan 15", 5, new DateTime(1993, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "hans.k@example.com", "Hans", "Kramer", "0690123456", 3, new DateTime(2025, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 10, "Zorgcentrum 4", 7, new DateTime(1972, 10, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "irene.vd@example.com", "Irene", "Van Dam", "0601234567", 4, new DateTime(2025, 4, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 11, "Gezondheidsstraat 2", 6, new DateTime(1980, 8, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "jeroen.f@example.com", "Jeroen", "Faber", "0611223344", 3, new DateTime(2025, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 12, "Pijnbestrijding 1", 5, new DateTime(1997, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "kim.s@example.com", "Kim", "Schouten", "0622334455", 4, new DateTime(2025, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 13, "Welzijnshof 9", 7, new DateTime(1960, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "lars.v@example.com", "Lars", "Veenstra", "0633445566", 3, new DateTime(2025, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 14, "Bewegingstraat 6", 6, new DateTime(1990, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "mieke.b@example.com", "Mieke", "Bos", "0644556677", 4, new DateTime(2025, 4, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 15, "Vitaliteitslaan 11", 5, new DateTime(1978, 7, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "niels.d@example.com", "Niels", "Dekker", "0655667788", 3, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 16, "Herstelstraat 14", 7, new DateTime(1983, 9, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "olga.p@example.com", "Olga", "Postma", "0666778899", 4, new DateTime(2025, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 17, "Zorgpad 1", 6, new DateTime(1992, 11, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "paul.k@example.com", "Paul", "Kuipers", "0677889900", 3, new DateTime(2025, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 18, "Medicijnlaan 3", 5, new DateTime(1975, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "quinn.v@example.com", "Quinn", "Vries", "0688990011", 4, new DateTime(2025, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 19, "Oefenplein 5", 7, new DateTime(1987, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "ruben.s@example.com", "Ruben", "Smeets", "0699001122", 3, new DateTime(2025, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 20, "Genezingsweg 10", 6, new DateTime(1994, 8, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "sarah.k@example.com", "Sarah", "Koks", "0610203040", 4, new DateTime(2025, 5, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 21, "Sportlaan 2", 5, new DateTime(1968, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "tim.v@example.com", "Tim", "Visser", "0620304050", 3, new DateTime(2025, 5, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 22, "Kwaliteitsstraat 7", 7, new DateTime(1981, 6, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "ursula.b@example.com", "Ursula", "Bouwman", "0630405060", 4, new DateTime(2025, 5, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 23, "Zorglaan 9", 6, new DateTime(1996, 3, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "vincent.m@example.com", "Vincent", "Meijerink", "0640506070", 3, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 24, "Therapieplein 12", 5, new DateTime(1979, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "wendy.vdv@example.com", "Wendy", "van der Velde", "0650607080", 4, new DateTime(2025, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 25, "Revalidatiepad 15", 7, new DateTime(1986, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "xavier.dr@example.com", "Xavier", "De Ruiter", "0660708090", 3, new DateTime(2025, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 }
                });

            migrationBuilder.InsertData(
                table: "AccessoryAdvices",
                columns: new[] { "Id", "AdviceDate", "ExpectedUsagePeriod", "HuisartsId", "Name", "PatientId", "Status" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "6 weken", 3, "Krukken", 1, "Actief" },
                    { 2, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "12 weken", 3, "Kniebrace", 1, "Actief" },
                    { 3, new DateTime(2025, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "8 weken", 4, "Rugbrace", 2, "Actief" },
                    { 4, new DateTime(2025, 1, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "4 weken", 3, "Schouderbrace", 3, "Afgerond" },
                    { 5, new DateTime(2025, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "6 weken", 4, "Enkelbrace", 4, "Actief" },
                    { 6, new DateTime(2025, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "8 weken", 3, "Nekbrace", 5, "Actief" },
                    { 7, new DateTime(2025, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Onbepaald", 4, "Fysiotherapie banden", 6, "Actief" },
                    { 8, new DateTime(2025, 3, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "10 weken", 3, "Meniscusbrace", 7, "Actief" },
                    { 9, new DateTime(2025, 3, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Onbepaald", 4, "TENS apparaat", 8, "Actief" },
                    { 10, new DateTime(2025, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "4 weken", 3, "Polsbrace", 9, "Afgerond" },
                    { 11, new DateTime(2025, 4, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "8 weken", 4, "Kniebrace", 10, "Actief" },
                    { 12, new DateTime(2025, 4, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Onbepaald", 3, "Rugkussen", 11, "Actief" },
                    { 13, new DateTime(2025, 4, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "6 weken", 4, "Enkelgewichtjes", 12, "Actief" },
                    { 14, new DateTime(2025, 4, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "12 weken", 3, "Looprek", 13, "Actief" },
                    { 15, new DateTime(2025, 4, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "8 weken", 4, "Schouderkatrol", 14, "Actief" },
                    { 16, new DateTime(2025, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "6 weken", 3, "Knieband", 15, "Actief" },
                    { 17, new DateTime(2025, 5, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Onbepaald", 4, "Enkelfles", 16, "Actief" },
                    { 18, new DateTime(2025, 5, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "4 weken", 3, "Krukken", 17, "Actief" },
                    { 19, new DateTime(2025, 5, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "8 weken", 4, "Rugbrace", 18, "Actief" },
                    { 20, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "6 weken", 3, "Schouderriem", 19, "Actief" },
                    { 21, new DateTime(2025, 5, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "4 weken", 4, "Enkelondersteuning", 20, "Actief" },
                    { 22, new DateTime(2025, 5, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "10 weken", 3, "Kniebrace", 21, "Actief" },
                    { 23, new DateTime(2025, 5, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Onbepaald", 4, "Hielspoorzooltjes", 22, "Actief" },
                    { 24, new DateTime(2025, 6, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "12 weken", 3, "Rolstoel", 23, "Actief" },
                    { 25, new DateTime(2025, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Onbepaald", 4, "Handtrainer", 24, "Actief" }
                });

            migrationBuilder.InsertData(
                table: "ActivityLogs",
                columns: new[] { "Id", "Activity", "PatientId", "Timestamp" },
                values: new object[,]
                {
                    { 1, "Rustdag na operatie.", 1, new DateTime(2025, 1, 10, 12, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "Korte wandeling met krukken rondom huis.", 1, new DateTime(2025, 1, 11, 15, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "Huishoudelijke taken (afwas).", 1, new DateTime(2025, 1, 12, 10, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "Boodschappen gedaan met hulp.", 1, new DateTime(2025, 1, 12, 16, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, "30 minuten fysiotherapie oefeningen gedaan.", 1, new DateTime(2025, 1, 13, 11, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, "Korte fietstocht op hometrainer (15 min).", 1, new DateTime(2025, 1, 13, 17, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, "Oefeningen volgens schema uitgevoerd.", 1, new DateTime(2025, 1, 14, 9, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8, "Koken en voorbereiden maaltijden.", 1, new DateTime(2025, 1, 14, 14, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 9, "Oefeningen gedaan", 1, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 10, "Wandeling buiten", 1, new DateTime(2025, 1, 15, 16, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 11, "Eerste lichte oefeningen voor rug.", 2, new DateTime(2025, 2, 1, 14, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 12, "Half uur gefietst op hometrainer.", 2, new DateTime(2025, 2, 2, 9, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 13, "Korte wandeling buiten", 2, new DateTime(2025, 2, 2, 18, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 14, "Rug oefeningen gedaan", 2, new DateTime(2025, 2, 3, 10, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 15, "Wandeling met hond.", 2, new DateTime(2025, 2, 3, 15, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 16, "Oefeningen voor schouder mobiliteit.", 3, new DateTime(2025, 1, 16, 9, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 17, "Lichte huishoudelijke taken.", 3, new DateTime(2025, 1, 17, 11, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 18, "Enkel oefeningen volgens schema.", 4, new DateTime(2025, 2, 6, 10, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 19, "Korte wandeling binnenshuis.", 4, new DateTime(2025, 2, 7, 14, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 20, "Nekoefeningen met elastiek.", 5, new DateTime(2025, 3, 3, 10, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 21, "Rustig gewandeld.", 5, new DateTime(2025, 3, 4, 15, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "Id", "AppointmentDateTime", "DoctorId", "DurationMinutes", "PatientId", "Status", "Type" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 10, 14, 0, 0, 0, DateTimeKind.Unspecified), 5, 60, 1, "Afgerond", "Intake" },
                    { 2, new DateTime(2025, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), 5, 45, 1, "Afgerond", "Controle" },
                    { 3, new DateTime(2025, 2, 21, 11, 0, 0, 0, DateTimeKind.Unspecified), 5, 45, 1, "Afgerond", "Controle" },
                    { 4, new DateTime(2025, 3, 14, 13, 0, 0, 0, DateTimeKind.Unspecified), 5, 30, 1, "Afgerond", "Controle" },
                    { 5, new DateTime(2025, 4, 4, 9, 0, 0, 0, DateTimeKind.Unspecified), 5, 30, 1, "Gepland", "Controle" },
                    { 6, new DateTime(2025, 4, 25, 10, 0, 0, 0, DateTimeKind.Unspecified), 5, 30, 1, "Gepland", "Controle" },
                    { 7, new DateTime(2025, 2, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), 6, 60, 2, "Afgerond", "Intake" },
                    { 8, new DateTime(2025, 2, 22, 14, 0, 0, 0, DateTimeKind.Unspecified), 6, 45, 2, "Afgerond", "Controle" },
                    { 9, new DateTime(2025, 3, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), 6, 45, 2, "Gepland", "Gepland" },
                    { 10, new DateTime(2025, 4, 5, 11, 0, 0, 0, DateTimeKind.Unspecified), 6, 30, 2, "Gepland", "Controle" },
                    { 11, new DateTime(2025, 1, 15, 16, 0, 0, 0, DateTimeKind.Unspecified), 5, 60, 3, "Afgerond", "Intake" },
                    { 12, new DateTime(2025, 2, 5, 15, 0, 0, 0, DateTimeKind.Unspecified), 5, 45, 3, "Afgerond", "Controle" },
                    { 13, new DateTime(2025, 2, 26, 14, 0, 0, 0, DateTimeKind.Unspecified), 5, 30, 3, "Gepland", "Controle" },
                    { 14, new DateTime(2025, 2, 5, 10, 0, 0, 0, DateTimeKind.Unspecified), 7, 60, 4, "Afgerond", "Intake" },
                    { 15, new DateTime(2025, 2, 26, 11, 0, 0, 0, DateTimeKind.Unspecified), 7, 45, 4, "Afgerond", "Controle" },
                    { 16, new DateTime(2025, 3, 19, 13, 0, 0, 0, DateTimeKind.Unspecified), 7, 30, 4, "Gepland", "Controle" },
                    { 17, new DateTime(2025, 3, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), 6, 60, 5, "Afgerond", "Intake" },
                    { 18, new DateTime(2025, 3, 22, 10, 0, 0, 0, DateTimeKind.Unspecified), 6, 45, 5, "Gepland", "Controle" },
                    { 19, new DateTime(2025, 2, 10, 14, 0, 0, 0, DateTimeKind.Unspecified), 5, 60, 6, "Afgerond", "Intake" },
                    { 20, new DateTime(2025, 3, 3, 15, 0, 0, 0, DateTimeKind.Unspecified), 5, 45, 6, "Gepland", "Controle" }
                });

            migrationBuilder.InsertData(
                table: "ExerciseAssignments",
                columns: new[] { "Id", "CheckedOffAt", "ClientCheckedOff", "EndDate", "ExerciseId", "Frequency", "Notes", "PatientId", "Repetitions", "Sets", "StartDate" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 13, 18, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 3, "Focus op stabiliteit", 1, 10, 3, new DateTime(2025, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2025, 1, 13, 9, 15, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 3, "Hamstring flexibiliteit", 1, 15, 3, new DateTime(2025, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, new DateTime(2025, 1, 14, 9, 30, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 3, "Kuitspierversterking", 1, 12, 3, new DateTime(2025, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, new DateTime(2025, 1, 23, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 1, "Houd 30 sec vast", 1, 1, 3, new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, new DateTime(2025, 3, 3, 9, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 2, "Verhoging intensiteit", 1, 12, 4, new DateTime(2025, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, new DateTime(2025, 3, 3, 18, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 3, "Verdieping stretch", 1, 20, 3, new DateTime(2025, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, new DateTime(2025, 3, 3, 9, 15, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 3, "Lichte schouderoefeningen voor algehele conditie", 1, 10, 3, new DateTime(2025, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8, new DateTime(2025, 3, 5, 9, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 2, "Laatste fase knieherstel", 1, 15, 4, new DateTime(2025, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 9, new DateTime(2025, 4, 6, 9, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 1, "Core versterking gevorderd", 1, 1, 4, new DateTime(2025, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 10, new DateTime(2025, 4, 11, 9, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 3, "Meer explosieve kuitbewegingen", 1, 15, 3, new DateTime(2025, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 11, new DateTime(2025, 2, 4, 9, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 3, "Voorzichtige beweging", 2, 8, 3, new DateTime(2025, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 12, new DateTime(2025, 2, 4, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 2, "Houd 20 sec vast", 2, 1, 2, new DateTime(2025, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 13, new DateTime(2025, 3, 2, 9, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1, "Verbeteren flexibiliteit onderrug", 2, 10, 3, new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 14, new DateTime(2025, 4, 3, 9, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, "Verhoging belasting", 2, 10, 3, new DateTime(2025, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 15, new DateTime(2025, 4, 16, 9, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 2, "Enkelstabiliteit", 2, 10, 2, new DateTime(2025, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 16, new DateTime(2025, 1, 18, 9, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 3, "Focus op rotatie", 3, 15, 3, new DateTime(2025, 1, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 17, new DateTime(2025, 2, 2, 9, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, "Algemene conditie", 3, 10, 2, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 18, new DateTime(2025, 3, 6, 9, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 2, "Schouderflexibiliteit", 3, 12, 3, new DateTime(2025, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 19, new DateTime(2025, 4, 2, 9, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 1, "Core stabiliteit", 4, 1, 3, new DateTime(2025, 1, 4, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 20, new DateTime(2025, 2, 8, 9, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 2, "Enkelversterking", 4, 12, 3, new DateTime(2025, 2, 7, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 21, new DateTime(2025, 2, 8, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, "Lichte knieoefeningen ter ondersteuning", 4, 8, 2, new DateTime(2025, 2, 7, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 22, new DateTime(2025, 4, 3, 9, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 3, "Hamstring stretch voor balans", 4, 10, 3, new DateTime(2025, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 23, new DateTime(2025, 4, 11, 9, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 1, "Schouder mobiliteit", 4, 10, 3, new DateTime(2025, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 24, new DateTime(2025, 3, 4, 9, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, "Nek- en rugoefeningen", 5, 8, 3, new DateTime(2025, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 25, new DateTime(2025, 3, 4, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 1, "Nekstabiliteit", 5, 1, 2, new DateTime(2025, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 26, new DateTime(2025, 4, 6, 9, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1, "Arm- en schouderstretch", 5, 10, 3, new DateTime(2025, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 27, new DateTime(2025, 2, 13, 9, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, "Rugversterking", 6, 10, 3, new DateTime(2025, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 28, new DateTime(2025, 2, 13, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1, "Onderrug stretch", 6, 10, 3, new DateTime(2025, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 29, new DateTime(2025, 3, 2, 9, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 1, "Core voor rug", 6, 1, 3, new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 30, new DateTime(2025, 3, 8, 9, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, "Kniebuiging lichte belasting", 7, 10, 3, new DateTime(2025, 3, 7, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 31, new DateTime(2025, 3, 8, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1, "Hamstring flexibiliteit knie", 7, 15, 3, new DateTime(2025, 3, 7, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 32, new DateTime(2025, 3, 21, 9, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 2, "Kuitspieren en balans", 7, 12, 3, new DateTime(2025, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 33, new DateTime(2025, 4, 16, 9, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 1, "Algemene core stability", 8, 1, 2, new DateTime(2025, 3, 4, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 34, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 1, "Lichte mobiliteitsoefeningen", 8, 10, 3, new DateTime(2025, 3, 4, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 35, new DateTime(2025, 1, 16, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, "Algemene krachtopbouw", 9, 10, 3, new DateTime(2025, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 36, new DateTime(2025, 1, 17, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 1, "Kuit- en enkelstabiliteit", 9, 12, 3, new DateTime(2025, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 37, new DateTime(2025, 1, 18, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, "Lichte kniebuigingen", 10, 8, 3, new DateTime(2025, 4, 7, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 38, new DateTime(2025, 1, 19, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1, "Stretch voor mobiliteit", 10, 10, 3, new DateTime(2025, 4, 7, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 39, new DateTime(2025, 1, 20, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 1, "Core stability oefeningen", 11, 1, 3, new DateTime(2025, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 40, new DateTime(2025, 1, 21, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 1, "Schouder en nek mobiliteit", 11, 10, 2, new DateTime(2025, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 41, new DateTime(2025, 1, 22, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, "Lichte belasting oefeningen", 12, 10, 3, new DateTime(2025, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 42, new DateTime(2025, 1, 23, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1, "Flexibiliteit", 12, 15, 3, new DateTime(2025, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 43, new DateTime(2025, 1, 24, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, "Knieversterking", 13, 8, 3, new DateTime(2025, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 44, new DateTime(2025, 1, 25, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 1, "Kuitspier activatie", 13, 12, 3, new DateTime(2025, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 45, new DateTime(2025, 1, 26, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 2, "Algemene stretch", 14, 10, 3, new DateTime(2025, 4, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 46, new DateTime(2025, 1, 27, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 1, "Core stability algemeen", 14, 1, 3, new DateTime(2025, 4, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 47, new DateTime(2025, 1, 28, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, "Start herstel", 15, 10, 3, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 48, new DateTime(2025, 1, 29, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1, "Flexibiliteit", 15, 15, 3, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 49, new DateTime(2025, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 2, "Kuit en enkel", 16, 12, 3, new DateTime(2025, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 50, new DateTime(2025, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 1, "Core training", 16, 1, 3, new DateTime(2025, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 51, new DateTime(2025, 2, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, "Lichte kracht", 17, 10, 3, new DateTime(2025, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 52, new DateTime(2025, 2, 2, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 1, "Schouder mobiliteit", 17, 10, 3, new DateTime(2025, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 53, new DateTime(2025, 2, 3, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 2, "Algemene stretch", 18, 15, 3, new DateTime(2025, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 54, new DateTime(2025, 2, 4, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 1, "Kuit en balans", 18, 12, 3, new DateTime(2025, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 55, new DateTime(2025, 2, 5, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 1, "Core stability", 19, 1, 3, new DateTime(2025, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 56, new DateTime(2025, 2, 6, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, "Lichte knie", 19, 10, 3, new DateTime(2025, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 57, new DateTime(2025, 2, 7, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 1, "Schouder en nek", 20, 10, 3, new DateTime(2025, 5, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 58, new DateTime(2025, 2, 8, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 2, "Hamstring", 20, 15, 3, new DateTime(2025, 5, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 59, new DateTime(2025, 2, 9, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 1, "Kuitversterking", 21, 12, 3, new DateTime(2025, 5, 28, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 60, new DateTime(2025, 2, 10, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, "Kniebuigingen", 21, 10, 3, new DateTime(2025, 5, 28, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 61, new DateTime(2025, 2, 11, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 1, "Core oefeningen", 22, 1, 3, new DateTime(2025, 5, 30, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 62, new DateTime(2025, 2, 12, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 2, "Flexibiliteit", 22, 10, 3, new DateTime(2025, 5, 30, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 63, new DateTime(2025, 2, 13, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 1, "Schouder", 23, 10, 3, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 64, new DateTime(2025, 2, 14, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, "Knie", 23, 10, 3, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 65, new DateTime(2025, 2, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 2, "Hamstring", 24, 15, 3, new DateTime(2025, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 66, new DateTime(2025, 2, 16, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 1, "Kuit", 24, 12, 3, new DateTime(2025, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 67, new DateTime(2025, 2, 17, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 1, "Core", 25, 1, 3, new DateTime(2025, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 68, new DateTime(2025, 2, 18, 10, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 3, "Schouder", 25, 10, 3, new DateTime(2025, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "IntakeRecords",
                columns: new[] { "Id", "Date", "Diagnosis", "DoctorId", "Goals", "PatientId", "Severity" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gescheurde kruisbanden (ACL)", 5, "Volledig herstel kniefunctie; wondgenezing; pijnmanagement", 1, "Ernstig" },
                    { 2, new DateTime(2025, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hernia L5-S1", 6, "Pijnreductie; mobiliteit; terugkeer naar werk", 2, "Matig" },
                    { 3, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Schouder impingement", 5, "Pijnvrij bewegen; krachtopbouw", 3, "Licht tot matig" },
                    { 4, new DateTime(2025, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Enkelverstuiking graad 2", 7, "Stabiliteit; belasting opbouw", 4, "Matig" },
                    { 5, new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nekhernia C5-C6", 6, "Nekmobiliteit; zenuwherstel; pijncontrole", 5, "Ernstig" },
                    { 6, new DateTime(2025, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chronische rugpijn", 5, "Activatie; coping mechanismen; functiebehoud", 6, "Matig" },
                    { 7, new DateTime(2025, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gescheurde meniscus", 7, "Volledig herstel kniefunctie; pijnvrij sporten", 7, "Ernstig" },
                    { 8, new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Fibromyalgie", 6, "Pijnmanagement; energieniveau; levenskwaliteit", 8, "Chronisch" },
                    { 9, new DateTime(2025, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Carpale tunnelsyndroom", 5, "Pijnreductie; grijpkracht herstel", 9, "Licht" },
                    { 10, new DateTime(2025, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Knieartrose", 7, "Mobiliteit; pijnvermindering; spierversterking", 10, "Matig" }
                });

            migrationBuilder.InsertData(
                table: "Medication",
                columns: new[] { "Id", "Dosage", "EndDate", "Frequency", "HuisartsId", "Name", "PatientId", "StartDate", "Status" },
                values: new object[,]
                {
                    { 1, "500mg", new DateTime(2025, 1, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "3x daags", 3, "Amoxicilline", 1, new DateTime(2025, 1, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Afgerond" },
                    { 2, "500mg", new DateTime(2025, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "4x daags max", 3, "Paracetamol", 1, new DateTime(2025, 1, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Afgerond" },
                    { 3, "50mg", new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "2x daags", 4, "Diclofenac", 2, new DateTime(2025, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Afgerond" },
                    { 4, "400mg", new DateTime(2025, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "3x daags", 3, "Ibuprofen", 3, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Afgerond" },
                    { 5, "250mg", new DateTime(2025, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "2x daags", 4, "Naproxen", 4, new DateTime(2025, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Afgerond" },
                    { 6, "50mg", new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "1x daags", 3, "Tramadol", 5, new DateTime(2025, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Actief" },
                    { 7, "12mcg/u", new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Elke 72 uur", 4, "Fentanyl pleister", 6, new DateTime(2025, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Actief" },
                    { 8, "10mg", new DateTime(2025, 4, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "3x daags", 3, "Pijnstiller X", 7, new DateTime(2025, 3, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Afgerond" },
                    { 9, "75mg", new DateTime(2025, 6, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "2x daags", 4, "Lyrica", 8, new DateTime(2025, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Actief" },
                    { 10, "10mg", new DateTime(2025, 3, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "1x daags", 3, "Corticosteroïden", 9, new DateTime(2025, 3, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Afgerond" },
                    { 11, "Topisch", new DateTime(2025, 5, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "2x daags", 4, "Diclofenac gel", 10, new DateTime(2025, 4, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Actief" },
                    { 12, "5mg", new DateTime(2025, 5, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "1x daags", 3, "Spierverslapper Y", 11, new DateTime(2025, 4, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Actief" },
                    { 13, "20mg", new DateTime(2025, 5, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "2x daags", 4, "Pijnstiller Z", 12, new DateTime(2025, 4, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Actief" },
                    { 14, "50mg", new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "1x daags", 3, "Tramadol", 13, new DateTime(2025, 4, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Actief" },
                    { 15, "500mg", new DateTime(2025, 5, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "4x daags", 4, "Paracetamol", 14, new DateTime(2025, 4, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Actief" },
                    { 16, "400mg", new DateTime(2025, 6, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "3x daags", 3, "Ibuprofen", 15, new DateTime(2025, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Actief" },
                    { 17, "250mg", new DateTime(2025, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "2x daags", 4, "Naproxen", 16, new DateTime(2025, 5, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Actief" },
                    { 18, "30mg", new DateTime(2025, 6, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "2x daags", 3, "Codeine", 17, new DateTime(2025, 5, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Actief" },
                    { 19, "500mg", new DateTime(2025, 6, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "4x daags", 4, "Paracetamol", 18, new DateTime(2025, 5, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Actief" },
                    { 20, "50mg", new DateTime(2025, 6, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "2x daags", 3, "Diclofenac", 19, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Actief" },
                    { 21, "400mg", new DateTime(2025, 6, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "3x daags", 4, "Ibuprofen", 20, new DateTime(2025, 5, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Actief" },
                    { 22, "500mg", new DateTime(2025, 6, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "4x daags", 3, "Paracetamol", 21, new DateTime(2025, 5, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "Actief" },
                    { 23, "250mg", new DateTime(2025, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "2x daags", 4, "Naproxen", 22, new DateTime(2025, 5, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Actief" },
                    { 24, "50mg", new DateTime(2025, 7, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "1x daags", 3, "Tramadol", 23, new DateTime(2025, 6, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Actief" },
                    { 25, "10mg", new DateTime(2025, 7, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "3x daags", 4, "Pijnstiller A", 24, new DateTime(2025, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Actief" }
                });

            migrationBuilder.InsertData(
                table: "PainEntries",
                columns: new[] { "Id", "Note", "PatientId", "Score", "Timestamp" },
                values: new object[,]
                {
                    { 1, "Na de operatie", 1, 7, new DateTime(2025, 1, 10, 10, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "lets minder pijn", 1, 6, new DateTime(2025, 1, 11, 9, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "Pijn bij bewegen", 1, 5, new DateTime(2025, 1, 12, 9, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "Tijdens oefeningen wat meer pijn", 1, 5, new DateTime(2025, 1, 13, 9, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, "Begin van de dag goed", 1, 4, new DateTime(2025, 1, 14, 9, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, "Pijn nam licht toe na intensievere oefening", 1, 4, new DateTime(2025, 1, 15, 9, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, "Gaat steeds beter", 1, 3, new DateTime(2025, 1, 16, 9, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8, "Alleen nog lichte pijn bij langdurig staan", 1, 3, new DateTime(2025, 1, 17, 9, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 9, "Bijna pijnvrij in rust", 1, 2, new DateTime(2025, 1, 18, 9, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 10, "Soms een steek bij onverwachte beweging", 1, 2, new DateTime(2025, 1, 19, 9, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 11, "Erge uitstralende pijn in been", 2, 8, new DateTime(2025, 2, 1, 11, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 12, "Medicatie helpt iets", 2, 7, new DateTime(2025, 2, 2, 10, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 13, "Na fysiotherapie beter", 2, 6, new DateTime(2025, 2, 3, 9, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 14, "Pijn blijft hoog na lange zit", 2, 6, new DateTime(2025, 2, 4, 9, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 15, "Iets beter na meer rust", 2, 5, new DateTime(2025, 2, 5, 9, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 16, "Zeurende pijn in schouder", 3, 6, new DateTime(2025, 1, 15, 14, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 17, "Pijn bij bovenhandse bewegingen", 3, 5, new DateTime(2025, 1, 16, 10, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 18, "Flink gezwollen enkel", 4, 7, new DateTime(2025, 2, 5, 13, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 19, "Minder zwelling", 4, 6, new DateTime(2025, 2, 6, 9, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 20, "Ondraaglijke nekpijn en tintelingen", 5, 9, new DateTime(2025, 3, 1, 16, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 21, "Medicatie helpt", 5, 8, new DateTime(2025, 3, 2, 9, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "PatientNotes",
                columns: new[] { "Id", "AuthorUserId", "Content", "PatientId", "Timestamp" },
                values: new object[,]
                {
                    { 1, 3, "Wondinfectie arm lijkt te verbeteren na start antibiotica.", 1, new DateTime(2025, 1, 9, 16, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 5, "Patiënt is gemotiveerd", 1, new DateTime(2025, 1, 10, 15, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 3, "Wondgenezing goed", 1, new DateTime(2025, 1, 20, 10, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, 5, "Voortgang positief", 1, new DateTime(2025, 1, 31, 10, 45, 0, 0, DateTimeKind.Unspecified) },
                    { 5, 4, "Patiënt klaagt over zenuwpijn", 2, new DateTime(2025, 2, 5, 9, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 6, 6, "Zenuwpijn besproken", 2, new DateTime(2025, 2, 6, 11, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, 3, "Schouderklachten lijken te stabiliseren.", 3, new DateTime(2025, 1, 17, 14, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 8, 5, "Oefenplan opgesteld voor schouder mobiliteit.", 3, new DateTime(2025, 1, 17, 17, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 9, 4, "Enkel nog steeds gezwollen", 4, new DateTime(2025, 2, 7, 10, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 10, 7, "Start oefenplan voor enkelstabiliteit.", 4, new DateTime(2025, 2, 7, 11, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 11, 3, "Nekpijn nog ernstig", 5, new DateTime(2025, 3, 3, 10, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 12, 6, "Intake afgerond", 5, new DateTime(2025, 3, 3, 10, 45, 0, 0, DateTimeKind.Unspecified) },
                    { 13, 4, "Chronische rugpijn besproken", 6, new DateTime(2025, 2, 12, 14, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 14, 5, "Activatieprogramma gestart voor rugpijn.", 6, new DateTime(2025, 2, 12, 15, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 15, 3, "Meniscus scheur", 7, new DateTime(2025, 3, 7, 10, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 16, 7, "Focus op herstel kniefunctie voor sportterugkeer.", 7, new DateTime(2025, 3, 7, 11, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 17, 4, "Fibromyalgie patiënt", 8, new DateTime(2025, 4, 3, 10, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 18, 6, "Behandelplan gericht op energiebeheer en pijnreductie.", 8, new DateTime(2025, 4, 3, 11, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 19, 3, "CT-scan uitslag besproken", 9, new DateTime(2025, 3, 12, 10, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 20, 5, "Start fysiotherapie voor pols en hand.", 9, new DateTime(2025, 3, 12, 11, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessoryAdvices_PatientId",
                table: "AccessoryAdvices",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_PatientId",
                table: "ActivityLogs",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientId",
                table: "Appointments",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_PatientId",
                table: "AuditLogs",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseAssignments_ExerciseId",
                table: "ExerciseAssignments",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseAssignments_PatientId",
                table: "ExerciseAssignments",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_IntakeRecords_PatientId",
                table: "IntakeRecords",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Medication_PatientId",
                table: "Medication",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PainEntries_PatientId",
                table: "PainEntries",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientNotes_PatientId",
                table: "PatientNotes",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_AssignedDoctorUserId",
                table: "Patients",
                column: "AssignedDoctorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_ReferringDoctorUserId",
                table: "Patients",
                column: "ReferringDoctorUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessoryAdvices");

            migrationBuilder.DropTable(
                name: "ActivityLogs");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "ExerciseAssignments");

            migrationBuilder.DropTable(
                name: "IntakeRecords");

            migrationBuilder.DropTable(
                name: "Medication");

            migrationBuilder.DropTable(
                name: "PainEntries");

            migrationBuilder.DropTable(
                name: "PatientNotes");

            migrationBuilder.DropTable(
                name: "Exercises");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
