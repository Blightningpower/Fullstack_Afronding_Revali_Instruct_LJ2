using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RevaliInstruct.Core.Data;
using RevaliInstruct.Core.Entities;
using Microsoft.Extensions.Logging;

namespace RevaliInstruct.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        /// Haalt een lijst van patiënten op.
        /// Ondersteunt zoeken op naam (q) en filteren op status.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetPatients(
            [FromQuery] string? q,
            [FromQuery] string? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? sort = null)
        {
            var query = _context.Patients.AsQueryable();

            // (optioneel) later kun je hier filteren op ingelogde arts:
            // var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // if (int.TryParse(userIdClaim, out var doctorId))
            // {
            //     query = query.Where(p => p.AssignedDoctorUserId == doctorId);
            // }

            // Filter op naam (voor- of achternaam bevat q)
            if (!string.IsNullOrWhiteSpace(q))
            {
                var needle = q.Trim();
                query = query.Where(p =>
                    p.FirstName.Contains(needle) ||
                    p.LastName.Contains(needle));
            }

            // Filter op status (enum-naam, bv. "Active", "Completed", ...)
            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<PatientStatus>(status, ignoreCase: true, out var parsedStatus))
            {
                query = query.Where(p => p.Status == parsedStatus);
            }

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
                    p.AssignedDoctorUserId
                })
                .ToListAsync();

            return Ok(items);
        }

        /// <summary>
        /// Haalt de details op van één patiënt.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<object>> GetPatient(int id)
        {
            var p = await _context.Patients.FindAsync(id);
            if (p == null)
                return NotFound();

            return Ok(new
            {
                p.Id,
                p.FirstName,
                p.LastName,
                p.Status,
                p.StartDate,
                p.DateOfBirth,
                p.Diagnosis,
                p.Notes,
                p.AssignedDoctorUserId
            });
        }

        /// <summary>
        /// Haalt het dossier op van één patiënt.
        /// </summary>
        [HttpGet("{id:int}/dossier")]
        public async Task<ActionResult<object>> GetPatientDossier(int id)
        {
            try
            {
                // Alleen de Patient zelf ophalen; geen Includes naar niet-bestaande tabellen
                var patient = await _context.Patients
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (patient == null)
                    return NotFound(new { message = $"Patient {id} not found" });

                // Simpel dossier – genoeg voor US1
                var dossier = new
                {
                    patient.Id,
                    patient.FirstName,
                    patient.LastName,
                    patient.Status,
                    patient.StartDate,
                    patient.DateOfBirth,
                    patient.Diagnosis,
                    patient.Notes
                    // Als je later Intakes / Exercises / PainEntries tabellen hebt,
                    // kun je die hier weer toevoegen.
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

        // ...optioneel: andere endpoints...
    }
}