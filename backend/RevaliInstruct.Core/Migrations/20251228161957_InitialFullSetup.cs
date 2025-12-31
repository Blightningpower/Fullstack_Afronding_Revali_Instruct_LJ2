using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RevaliInstruct.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialFullSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    Duration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessoryAdvices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActivityLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

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
                name: "PatientNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    AuthorId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientNotes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganisatieId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    ExerciseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseAssignments_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AssignedDoctorUserId = table.Column<int>(type: "int", nullable: false)
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
                name: "InvoiceItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceItems_Patients_PatientId",
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

            migrationBuilder.InsertData(
                table: "AccessoryAdvices",
                columns: new[] { "Id", "AdviceDate", "Duration", "HuisartsId", "Name", "PatientId", "Status" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "6 weken", 3, "Krukken", 1, "Actief" },
                    { 2, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "12 weken", 3, "Kniebrace", 1, "Actief" }
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
                    { 5, "Cirkelbewegingen met de armen.", "Schouder Rotaties", "https://www.youtube.com/watch?v=oLwTC-lAJws&list=PLurruTwy8-10c7tAfYnJRkFVy_U1O3IZ1" }
                });

            migrationBuilder.InsertData(
                table: "PatientNotes",
                columns: new[] { "Id", "AuthorId", "Content", "PatientId", "Timestamp" },
                values: new object[,]
                {
                    { 1, 3, "Wondinfectie arm lijkt te verbeteren na start antibiotica.", 1, new DateTime(2025, 1, 9, 16, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 5, "Patiënt is gemotiveerd", 1, new DateTime(2025, 1, 10, 15, 0, 0, 0, DateTimeKind.Unspecified) }
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
                columns: new[] { "Id", "Address", "AssignedDoctorUserId", "BirthDate", "Email", "FirstName", "LastName", "Notes", "Phone", "StartDate", "Status" },
                values: new object[,]
                {
                    { 1, "Kruisbandstraat 10", 3, new DateTime(1995, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "freddy.v@example.com", "Freddy", "Voetballer", null, null, new DateTime(2025, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, "Therapiestraat 5", 4, new DateTime(1988, 11, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "anna.k@example.com", "Anna", "Kamer", null, null, new DateTime(2025, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 3, "Oefenlaan 12", 3, new DateTime(1976, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "bas.v@example.com", "Bas", "Verkade", null, null, new DateTime(2025, 1, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 4, "Herstelweg 3", 4, new DateTime(2001, 1, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "carla.d@example.com", "Carla", "Dirksen", null, null, new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 5, "Revalidatieplein 7", 3, new DateTime(1965, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "dirk.m@example.com", "Dirk", "Meijer", null, null, new DateTime(2025, 2, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 6, "Fysioweg 22", 4, new DateTime(1999, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "eva.p@example.com", "Eva", "Peters", null, null, new DateTime(2025, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 7, "Gipsstraat 1", 3, new DateTime(1970, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "frank.dj@example.com", "Frank", "de Jong", null, null, new DateTime(2025, 3, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 8, "Rolstoelpad 8", 4, new DateTime(1985, 12, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "greetje.w@example.com", "Greetje", "Willems", null, null, new DateTime(2025, 3, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 9, "Sportlaan 15", 3, new DateTime(1993, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "hans.k@example.com", "Hans", "Kramer", null, null, new DateTime(2025, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 10, "Zorgcentrum 4", 4, new DateTime(1972, 10, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "irene.vd@example.com", "Irene", "Van Dam", null, null, new DateTime(2025, 4, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 11, "Gezondheidsstraat 2", 3, new DateTime(1980, 8, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "jeroen.f@example.com", "Jeroen", "Faber", null, null, new DateTime(2025, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 12, "Pijnbestrijding 1", 4, new DateTime(1997, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "kim.s@example.com", "Kim", "Schouten", null, null, new DateTime(2025, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 13, "Welzijnshof 9", 3, new DateTime(1960, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "lars.v@example.com", "Lars", "Veenstra", null, null, new DateTime(2025, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 14, "Bewegingstraat 6", 4, new DateTime(1990, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "mieke.b@example.com", "Mieke", "Bos", null, null, new DateTime(2025, 4, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 15, "Vitaliteitslaan 11", 3, new DateTime(1978, 7, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "niels.d@example.com", "Niels", "Dekker", null, null, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 16, "Herstelstraat 14", 4, new DateTime(1983, 9, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "olga.p@example.com", "Olga", "Postma", null, null, new DateTime(2025, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 17, "Zorgpad 1", 3, new DateTime(1992, 11, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "paul.k@example.com", "Paul", "Kuipers", null, null, new DateTime(2025, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 18, "Medicijnlaan 3", 4, new DateTime(1975, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "quinn.v@example.com", "Quinn", "Vries", null, null, new DateTime(2025, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 19, "Oefenplein 5", 3, new DateTime(1987, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "ruben.s@example.com", "Ruben", "Smeets", null, null, new DateTime(2025, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 20, "Genezingsweg 10", 4, new DateTime(1994, 8, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "sarah.k@example.com", "Sarah", "Koks", null, null, new DateTime(2025, 5, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 21, "Sportlaan 2", 3, new DateTime(1968, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "tim.v@example.com", "Tim", "Visser", null, null, new DateTime(2025, 5, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 22, "Kwaliteitsstraat 7", 4, new DateTime(1981, 6, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "ursula.b@example.com", "Ursula", "Bouwman", null, null, new DateTime(2025, 5, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 23, "Zorglaan 9", 3, new DateTime(1996, 3, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "vincent.m@example.com", "Vincent", "Meijerink", null, null, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 24, "Therapieplein 12", 4, new DateTime(1979, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "wendy.vdv@example.com", "Wendy", "van der Velde", null, null, new DateTime(2025, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 25, "Revalidatiepad 15", 3, new DateTime(1986, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "xavier.dr@example.com", "Xavier", "De Ruiter", null, null, new DateTime(2025, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 }
                });

            migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "Id", "AppointmentDateTime", "DoctorId", "DurationMinutes", "PatientId", "Status", "Type" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 10, 14, 0, 0, 0, DateTimeKind.Unspecified), 5, 60, 1, "Afgerond", "Intake" },
                    { 2, new DateTime(2025, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), 5, 45, 1, "Afgerond", "Controle" },
                    { 5, new DateTime(2025, 4, 4, 9, 0, 0, 0, DateTimeKind.Unspecified), 5, 30, 1, "Gepland", "Controle" }
                });

            migrationBuilder.InsertData(
                table: "IntakeRecords",
                columns: new[] { "Id", "Date", "Diagnosis", "DoctorId", "Goals", "PatientId", "Severity" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gescheurde kruisbanden (ACL)", 5, "Volledig herstel kniefunctie; wondgenezing; pijnmanagement", 1, "Ernstig" },
                    { 2, new DateTime(2025, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hernia L5-S1", 6, "Pijnreductie; mobiliteit; terugkeer naar werk", 2, "Matig" },
                    { 3, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Schouder impingement", 5, "Pijnvrij bewegen; krachtopbouw", 3, "Licht tot matig" }
                });

            migrationBuilder.InsertData(
                table: "InvoiceItems",
                columns: new[] { "Id", "Amount", "AuthorId", "Date", "Description", "PatientId", "Status", "Type" },
                values: new object[,]
                {
                    { 1, 3500m, 3, new DateTime(2025, 1, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Consult wondverzorging arm", 1, "Nieuw", "Huisarts" },
                    { 27, 8000m, 5, new DateTime(2025, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Intake revalidatie knie", 1, "Nieuw", "Revalidatiearts" }
                });

            migrationBuilder.InsertData(
                table: "PainEntries",
                columns: new[] { "Id", "Note", "PatientId", "Score", "Timestamp" },
                values: new object[,]
                {
                    { 1, "Na de operatie", 1, 7, new DateTime(2025, 1, 10, 10, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "Iets minder pijn", 1, 6, new DateTime(2025, 1, 11, 9, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "Pijn bij bewegen", 1, 5, new DateTime(2025, 1, 12, 9, 30, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientId",
                table: "Appointments",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseAssignments_ExerciseId",
                table: "ExerciseAssignments",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_IntakeRecords_PatientId",
                table: "IntakeRecords",
                column: "PatientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_PatientId",
                table: "InvoiceItems",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PainEntries_PatientId",
                table: "PainEntries",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_AssignedDoctorUserId",
                table: "Patients",
                column: "AssignedDoctorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
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
                name: "InvoiceItems");

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
