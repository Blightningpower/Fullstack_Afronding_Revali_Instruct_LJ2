using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using RevaliInstruct.Api.Controllers;
using RevaliInstruct.Api.Dtos;
using RevaliInstruct.Api.Services;
using RevaliInstruct.Core.Data;
using RevaliInstruct.Core.Entities;
using FluentAssertions;
using Xunit;

namespace RevaliInstruct.Tests
{
    public class PatientsControllerTests
    {
        private readonly Mock<ICurrentUserService> _mockUser;
        private readonly ApplicationDbContext _context;
        private readonly PatientsController _controller;

        public PatientsControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            _mockUser = new Mock<ICurrentUserService>();
            _mockUser.Setup(m => m.UserId).Returns(5); // Ingelogd als Arts #5

            _controller = new PatientsController(_context, _mockUser.Object);
        }

        #region US9: Toegangscontrole
        [Fact]
        public async Task GetPatient_OtherDoctorsPatient_ReturnsForbid()
        {
            _context.Patients.Add(new Patient { Id = 1, FirstName = "Test", AssignedDoctorUserId = 99 });
            await _context.SaveChangesAsync();

            var result = await _controller.GetPatient(1);

            result.Result.Should().BeOfType<ForbidResult>();
        }
        #endregion

        #region US3 & US10: Intake & Audit Trail
        [Fact]
        public async Task CreateIntake_ValidData_SavesAndLogs()
        {
            _context.Patients.Add(new Patient { Id = 10, AssignedDoctorUserId = 5 });
            await _context.SaveChangesAsync();

            var dto = new IntakeDto { Diagnosis = "Breuk", Severity = "Licht", InitialGoals = "Herstel" };

            var result = await _controller.CreateIntake(10, dto);

            result.Should().BeOfType<OkObjectResult>();
            var intake = await _context.IntakeRecords.FirstOrDefaultAsync(i => i.PatientId == 10);
            
            // Fix voor CS8602: Gebruik ! na controle
            intake.Should().NotBeNull();
            intake!.Diagnosis.Should().Be("Breuk");

            var log = await _context.AuditLogs.FirstOrDefaultAsync(l => l.TableName == "IntakeRecords");
            log.Should().NotBeNull();
            log!.RecordId.Should().Be(intake.Id);
            log.Action.Should().Be("INSERT");
        }
        #endregion

        #region US6: Afspraken
        [Fact]
        public async Task CancelAppointment_UpdatesStatus_AndLogs()
        {
            _context.Patients.Add(new Patient { Id = 1, AssignedDoctorUserId = 5 });
            var app = new Appointment { Id = 100, PatientId = 1, Status = "Gepland", Type = "Fysio" };
            _context.Appointments.Add(app);
            await _context.SaveChangesAsync();

            await _controller.CancelAppointment(1, 100);

            app.Status.Should().Be("Geannuleerd");
            var log = await _context.AuditLogs.FirstOrDefaultAsync(l => l.Action == "Afspraak Geannuleerd");
            log.Should().NotBeNull();
        }
        #endregion

        #region Declaraties & Notities
        [Fact]
        public async Task CreateDeclaration_NegativeAmount_ReturnsBadRequest()
        {
            _context.Patients.Add(new Patient { Id = 1, AssignedDoctorUserId = 5 });
            await _context.SaveChangesAsync();

            var result = await _controller.CreateDeclaration(1, new Declaration { Amount = -50 });

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task AddNote_SavesNote_WithCorrectAuditId()
        {
            _context.Patients.Add(new Patient { Id = 1, AssignedDoctorUserId = 5 });
            await _context.SaveChangesAsync();

            await _controller.AddNote(1, new PatientNoteDto { Content = "Checkup" });

            var note = await _context.PatientNotes.FirstOrDefaultAsync();
            note.Should().NotBeNull();
            
            var log = await _context.AuditLogs.FirstOrDefaultAsync(l => l.TableName == "PatientNotes");
            log.Should().NotBeNull();
            log!.RecordId.Should().Be(note!.Id); // Verifieert fix voor ID #0
        }
        #endregion
    }
}