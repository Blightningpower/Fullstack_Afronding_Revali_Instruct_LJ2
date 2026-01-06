using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RevaliInstruct.Core.Data;
using Microsoft.AspNetCore.Authorization;
using RevaliInstruct.Core.Entities;
using RevaliInstruct.Api.Dtos;
using RevaliInstruct.Api.Services;

namespace RevaliInstruct.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public PatientsController(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientListItemDto>>> GetPatients(
            [FromQuery] string? searchTerm,
            [FromQuery] string? status)
        {
            var currentUserId = _currentUserService.UserId;

            if (currentUserId == null)
            {
                return Unauthorized();
            }

            var query = _context.Patients
                .Where(p => p.AssignedDoctorUserId == currentUserId);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(p =>
                    p.FirstName.ToLower().Contains(term) ||
                    p.LastName.ToLower().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(status) && status != "Alle statussen")
            {
                query = query.Where(p => p.Status.ToString() == status);
            }

            var patients = await query
                .Select(p => new PatientListItemDto
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    StartDate = p.StartDate,
                    Status = p.Status,
                    StatusLabel = p.Status.ToString()
                })
                .ToListAsync();

            return Ok(patients);
        }

        [HttpGet("{id}/dossier")]
        public async Task<ActionResult<Patient>> GetPatient(int id)
        {
            var currentUserId = _currentUserService.UserId;

            if (currentUserId == null)
            {
                return Unauthorized();
            }

            var patient = await _context.Patients
                .Include(p => p.ReferringDoctor)
                .Include(p => p.Exercises)
                    .ThenInclude(e => e.Exercise)
                .Include(p => p.PainEntries)
                .Include(p => p.ActivityLogEntries)
                .Include(p => p.Medications)
                .Include(p => p.AccessoryAdvices)
                .Include(p => p.Appointments)
                .Include(p => p.IntakeRecords)
                .Include(p => p.Declarations)
                .Include(p => p.PatientNotes)
                    .ThenInclude(n => n.Author)
                .AsSplitQuery()
                .FirstOrDefaultAsync(p => p.Id == id && p.AssignedDoctorUserId == currentUserId);

            if (patient == null) return NotFound();

            return Ok(patient);
        }

        [HttpPost("{id}/intake")]
        public async Task<IActionResult> CreateIntake(int id, [FromBody] IntakeDto dto)
        {
            var currentUserId = _currentUserService.UserId;
            if (currentUserId == null) return Unauthorized();

            var patient = await _context.Patients
                .Include(p => p.IntakeRecords)
                .FirstOrDefaultAsync(p => p.Id == id && p.AssignedDoctorUserId == currentUserId);

            if (patient == null) return NotFound("Patiënt niet gevonden.");
            if (patient.IntakeRecords != null && patient.IntakeRecords.Any())
            {
                return BadRequest("Er bestaat al een intakeverslag voor deze patiënt.");
            }

            var intake = new IntakeRecord
            {
                PatientId = id,
                DoctorId = currentUserId.Value,
                Diagnosis = dto.Diagnosis,
                Severity = dto.Severity,
                Goals = dto.Goals,
                Date = DateTime.Now
            };

            _context.IntakeRecords.Add(intake);
            _context.AuditLogs.Add(new AuditLog
            {
                UserId = currentUserId.Value,
                Action = "Intake Geregistreerd",
                Timestamp = DateTime.Now,
                Details = $"Patiënt ID: {id}"
            });

            await _context.SaveChangesAsync();
            return Ok(intake);
        }

        [HttpPost("{id}/exercises")]
        public async Task<IActionResult> AssignExercise(int id, [FromBody] ExerciseAssignmentDto dto)
        {
            var currentUserId = _currentUserService.UserId;
            if (currentUserId == null) return Unauthorized();

            var patientExists = await _context.Patients
                .AnyAsync(p => p.Id == id && p.AssignedDoctorUserId == currentUserId);

            if (!patientExists) return NotFound("Patiënt niet gevonden of geen toegang.");

            var assignment = new ExerciseAssignment
            {
                PatientId = id,
                ExerciseId = dto.ExerciseId,
                Repetitions = dto.Repetitions,
                Sets = dto.Sets,
                Frequency = dto.Frequency,
                Notes = dto.Notes,
                StartDate = dto.StartDate ?? DateTime.Now,
                EndDate = dto.EndDate ?? DateTime.Now.AddMonths(1),
                ClientCheckedOff = false
            };

            _context.ExerciseAssignments.Add(assignment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Oefening succesvol toegewezen" });
        }

        [HttpPost("{id}/appointments")]
        public async Task<IActionResult> CreateAppointment(int id, [FromBody] Appointment appointment)
        {
            var currentUserId = _currentUserService.UserId;
            if (currentUserId == null) return Unauthorized();

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == id && p.AssignedDoctorUserId == currentUserId);
            if (patient == null) return NotFound("Patiënt niet gevonden.");

            appointment.PatientId = id;
            appointment.DoctorId = currentUserId.Value;
            appointment.Status = "Gepland";

            _context.Appointments.Add(appointment);

            _context.AuditLogs.Add(new AuditLog
            {
                UserId = currentUserId.Value,
                Action = "Afspraak Gepland",
                Timestamp = DateTime.Now,
                Details = $"Patiënt ID: {id}, Type: {appointment.Type}, Datum: {appointment.DateTime}"
            });

            await _context.SaveChangesAsync();
            return Ok(appointment);
        }

        [HttpPut("{id}/appointments/{appId}")]
        public async Task<IActionResult> UpdateAppointment(int id, int appId, [FromBody] Appointment dto)
        {
            var currentUserId = _currentUserService.UserId;
            if (currentUserId == null) return Unauthorized();

            var app = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == appId && a.PatientId == id);
            if (app == null) return NotFound("Afspraak niet gevonden.");

            app.DateTime = dto.DateTime;
            app.Duration = dto.Duration;
            app.Type = dto.Type;

            _context.AuditLogs.Add(new AuditLog
            {
                UserId = currentUserId.Value,
                Action = "Afspraak Gewijzigd",
                Timestamp = DateTime.Now,
                Details = $"Afspraak ID: {appId}, Nieuw type: {dto.Type}, Nieuwe datum: {dto.DateTime}"
            });

            await _context.SaveChangesAsync();
            return Ok(app);
        }

        [HttpPatch("{id}/appointments/{appId}/cancel")]
        public async Task<IActionResult> CancelAppointment(int id, int appId)
        {
            var currentUserId = _currentUserService.UserId;
            if (currentUserId == null) return Unauthorized();

            var app = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == appId && a.PatientId == id);
            if (app == null) return NotFound("Afspraak niet gevonden.");

            app.Status = "Geannuleerd";

            _context.AuditLogs.Add(new AuditLog
            {
                UserId = currentUserId.Value,
                Action = "Afspraak Geannuleerd",
                Timestamp = DateTime.Now,
                Details = $"Afspraak ID: {appId} geannuleerd voor patiënt {id}"
            });

            await _context.SaveChangesAsync();
            return Ok(new { message = "Afspraak succesvol geannuleerd." });
        }

        [HttpPost("{id}/declarations")]
        public async Task<IActionResult> CreateDeclaration(int id, [FromBody] Declaration dec)
        {
            var currentUserId = _currentUserService.UserId;
            if (currentUserId == null) return Unauthorized();

            if (dec.Amount < 0) return BadRequest("Bedrag mag niet negatief zijn.");

            dec.PatientId = id;
            dec.DoctorId = currentUserId.Value;
            dec.Date = DateTime.Now;
            dec.Status = "Geregistreerd";

            _context.Declarations.Add(dec);

            _context.AuditLogs.Add(new AuditLog
            {
                UserId = currentUserId.Value,
                Action = "Declaratie Geregistreerd",
                Timestamp = DateTime.Now,
                Details = $"Patiënt ID: {id}, Type: {dec.TreatmentType}, Bedrag: €{dec.Amount}"
            });

            await _context.SaveChangesAsync();
            return Ok(dec);
        }

        [HttpPatch("{id}/declarations/{decId}/mark-declared")]
        public async Task<IActionResult> MarkAsDeclared(int id, int decId)
        {
            var currentUserId = _currentUserService.UserId;
            if (currentUserId == null) return Unauthorized();

            var dec = await _context.Declarations.FirstOrDefaultAsync(d => d.Id == decId && d.PatientId == id);
            if (dec == null) return NotFound();

            dec.Status = "Gedeclareerd";

            _context.AuditLogs.Add(new AuditLog
            {
                UserId = currentUserId.Value,
                Action = "Status Declaratie Gewijzigd",
                Timestamp = DateTime.Now,
                Details = $"Declaratie ID: {decId} naar 'Gedeclareerd'"
            });

            await _context.SaveChangesAsync();
            return Ok(new { message = "Gemarkeerd als gedeclareerd" });
        }

        [HttpPost("{id}/notes")]
        public async Task<IActionResult> AddNote(int id, [FromBody] PatientNoteDto dto)
        {
            var currentUserId = _currentUserService.UserId;
            if (currentUserId == null) return Unauthorized();

            var note = new PatientNote
            {
                PatientId = id,
                AuthorUserId = currentUserId.Value,
                Timestamp = DateTime.Now, // Automatische tijdstempel
                Content = dto.Content
            };

            _context.PatientNotes.Add(note);

            // Registreer in Audit Trail (Eis US8)
            _context.AuditLogs.Add(new AuditLog
            {
                UserId = currentUserId.Value,
                Action = "Notitie Toegevoegd",
                Timestamp = DateTime.Now,
                Details = $"Nieuwe notitie voor patiënt {id}"
            });

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id}/notes/{noteId}")]
        public async Task<IActionResult> UpdateNote(int id, int noteId, [FromBody] string content)
        {
            var note = await _context.PatientNotes.FindAsync(noteId);
            if (note == null || note.AuthorUserId != _currentUserService.UserId) return Forbid();

            note.Content = content;
            note.Timestamp = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok(note);
        }

        [HttpDelete("{id}/notes/{noteId}")]
        public async Task<IActionResult> DeleteNote(int id, int noteId)
        {
            var note = await _context.PatientNotes.FindAsync(noteId);
            if (note == null || note.AuthorUserId != _currentUserService.UserId) return Forbid();

            _context.PatientNotes.Remove(note);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}