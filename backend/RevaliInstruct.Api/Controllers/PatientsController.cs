using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RevaliInstruct.Core.Data;
using Microsoft.AspNetCore.Authorization;
using RevaliInstruct.Core.Entities;
using RevaliInstruct.Api.Dtos;
using System.Linq;

namespace RevaliInstruct.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PatientsController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientListItemDto>>> GetPatients(
            [FromQuery] string? searchTerm,
            [FromQuery] string? status)
        {
            var userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
                        ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int currentUserId))
            {
                return Unauthorized();
            }

            var query = _context.Patients
                .Where(p => p.AssignedDoctorUserId == currentUserId);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                // Zoek in voornaam, achternaam of de gecombineerde naam
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
            var userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
                           ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int currentUserId))
            {
                return Unauthorized();
            }

            var patient = await _context.Patients
                .Include(p => p.ReferringDoctor)
                .Include(p => p.Exercises)
                    .ThenInclude(e => e.Exercise)
                .Include(p => p.PainEntries)
                .Include(p => p.ActivityLogs)
                .Include(p => p.Medications)
                .Include(p => p.AccessoryAdvices)
                .Include(p => p.Appointments)
                .Include(p => p.IntakeRecords)
                .Include(p => p.PatientNotes)
                .FirstOrDefaultAsync(p => p.Id == id && p.AssignedDoctorUserId == currentUserId);

            if (patient == null) return NotFound();

            return Ok(patient);
        }

        [HttpPost("{id}/intake")]
        public async Task<IActionResult> CreateIntake(int id, [FromBody] IntakeDto dto)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int currentUserId)) return Unauthorized();

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
                DoctorId = currentUserId,
                Diagnosis = dto.Diagnosis,
                Severity = dto.Severity,
                Goals = dto.Goals,
                Date = DateTime.Now
            };

            _context.IntakeRecords.Add(intake);
            _context.AuditLogs.Add(new AuditLog
            {
                UserId = currentUserId,
                Action = "Intake Geregistreerd",
                Timestamp = DateTime.Now,
                Details = $"Patiënt ID: {id}"
            });

            await _context.SaveChangesAsync();
            return Ok(intake);
        }

        [HttpPost("{id}/notes")]
        public async Task<IActionResult> AddNote(int id, [FromBody] PatientNote noteDto)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int currentUserId)) return Unauthorized();

            var note = new PatientNote
            {
                PatientId = id,
                AuthorUserId = currentUserId,
                Timestamp = DateTime.Now,
                Content = noteDto.Content
            };

            _context.PatientNotes.Add(note);
            await _context.SaveChangesAsync();
            return Ok(note);
        }
    }
}