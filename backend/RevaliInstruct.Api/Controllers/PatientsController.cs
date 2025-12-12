using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RevaliInstruct.Core.Data;
using RevaliInstruct.Core.Entities;

namespace RevaliInstruct.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]          // → /api/patients
    [Authorize(Roles = "Doctor,Admin")]
    public class PatientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PatientsController> _logger;

        public PatientsController(ApplicationDbContext context, ILogger<PatientsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Huidige user uit JWT ophalen (gebaseerd op Username in de Name-claim).
        /// </summary>
        private async Task<User?> GetCurrentUserAsync()
        {
            var username = User.Identity?.Name;

            if (string.IsNullOrWhiteSpace(username))
            {
                username = User.FindFirstValue(ClaimTypes.Name);
            }

            if (string.IsNullOrWhiteSpace(username))
                return null;

            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        /// <summary>
        /// Lijst van patiënten:
        /// - Doctor: alleen eigen patiënten (AssignedDoctorUserId == currentUser.Id)
        /// - Admin: alle patiënten
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetPatients(
            [FromQuery] string? q,
            [FromQuery] string? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? sort = null)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var query = _context.Patients.AsNoTracking().AsQueryable();

            // Doctor ziet alleen eigen patiënten
            if (currentUser.Role == "Doctor")
            {
                query = query.Where(p => p.AssignedDoctorUserId == currentUser.Id);
            }

            // Filter op naam
            if (!string.IsNullOrWhiteSpace(q))
            {
                var needle = q.Trim();
                query = query.Where(p =>
                    p.FirstName.Contains(needle) ||
                    p.LastName.Contains(needle));
            }

            // Filter op status (enumnaam)
            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<PatientStatus>(status, true, out var parsedStatus))
            {
                query = query.Where(p => p.Status == parsedStatus);
            }

            // Sorteren (eenvoudig)
            query = query.OrderBy(p => p.LastName).ThenBy(p => p.FirstName);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    p.Id,
                    p.FirstName,
                    p.LastName,
                    p.Status,
                    p.StartDate,
                    p.Notes,
                    p.AssignedDoctorUserId,
                    p.Email,
                    p.Phone,
                    p.ReferringDoctor
                })
                .ToListAsync();

            return Ok(items);
        }

        /// <summary>
        /// Details van één patiënt (zelfde beveiliging).
        /// </summary>
        [HttpGet("{id:int}/dossier")]
        public async Task<ActionResult<object>> GetPatientDossier(int id)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            try
            {
                var query = _context.Patients
                    .AsNoTracking()
                    .Include(p => p.ExerciseAssignments)
                        .ThenInclude(a => a.Exercise)
                    .Include(p => p.PainEntries)
                    .Include(p => p.ActivityLogs)
                    .Include(p => p.Appointments)
                    .Include(p => p.AccessoryAdvices)
                    // Intake/InvoiceItems/Notes kun je later altijd nog toevoegen
                    .Where(p => p.Id == id);

                if (currentUser.Role == "Doctor")
                {
                    query = query.Where(p => p.AssignedDoctorUserId == currentUser.Id);
                }

                var patient = await query.FirstOrDefaultAsync();
                if (patient == null)
                    return NotFound(new { message = $"Patient {id} not found or not accessible." });

                var dossier = new
                {
                    // basis
                    patient.Id,
                    patient.FirstName,
                    patient.LastName,
                    patient.Status,
                    patient.StartDate,
                    patient.DateOfBirth,
                    patient.Diagnosis,
                    patient.Notes,
                    patient.AssignedDoctorUserId,
                    patient.Email,
                    patient.Phone,
                    patient.ReferringDoctor,

                    // oefeningen
                    Exercises = patient.ExerciseAssignments.Select(a => new
                    {
                        a.Id,
                        ExerciseTitle = a.Exercise != null ? a.Exercise.Title : null,
                        a.Repetitions,
                        a.Sets,
                        a.Frequency,
                        a.Duration,
                        a.ClientCheckedOff,
                        a.StartDateUtc,
                        a.EndDateUtc
                    }).OrderBy(a => a.StartDateUtc),

                    // pijnindicaties
                    PainEntries = patient.PainEntries
                        .OrderBy(e => e.RecordedAtUtc)
                        .Select(e => new
                        {
                            e.Id,
                            e.RecordedAtUtc,
                            e.Score,
                            e.Location,
                            e.Note
                        }),

                    // activiteitenlogboek
                    ActivityLogs = patient.ActivityLogs
                        .OrderByDescending(l => l.LoggedAtUtc)
                        .Select(l => new
                        {
                            l.Id,
                            l.LoggedAtUtc,
                            l.Activity,
                            l.Details
                        }),

                    // medicatie & accessoires
                    AccessoryAdvices = patient.AccessoryAdvices
                        .OrderByDescending(a => a.AdviceDateUtc)
                        .Select(a => new
                        {
                            a.Id,
                            a.Name,
                            a.GPUserId,
                            a.AdviceDateUtc,
                            a.ExpectedUsagePeriod,
                            a.Status
                        }),

                    // afspraken
                    Appointments = patient.Appointments
                        .OrderByDescending(a => a.StartUtc)
                        .Select(a => new
                        {
                            a.Id,
                            a.StartUtc,
                            a.Duration,
                            a.Type,
                            a.Status
                        })
                };

                return Ok(dossier);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetPatientDossier for patient {PatientId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error while loading patient dossier", patientId = id });
            }
        }
    }
}