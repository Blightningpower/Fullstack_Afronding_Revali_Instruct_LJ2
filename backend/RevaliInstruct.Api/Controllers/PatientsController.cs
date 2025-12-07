using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RevaliInstruct.Core.Data;
using RevaliInstruct.Core.Entities;

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
        /// Helper: haalt de ingelogde user uit de JWT en zoekt hem op in de database.
        /// We gaan er hier vanuit dat de JWT het Username in ClaimTypes.Name stopt.
        /// </summary>
        private async Task<User?> GetCurrentUserAsync()
        {
            var username = User.Identity?.Name;

            if (string.IsNullOrWhiteSpace(username))
            {
                // fallback voor het geval de Name-claim anders is geconfigureerd
                username = User.FindFirstValue(ClaimTypes.Name);
            }

            if (string.IsNullOrWhiteSpace(username))
                return null;

            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        /// <summary>
        /// Haalt een lijst van patiënten op.
        /// Ondersteunt zoeken op naam (q) en filteren op status.
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

            var query = _context.Patients.AsQueryable();

            // 🔒 Dokter ziet alleen eigen patiënten
            if (currentUser.Role == "Doctor")
            {
                query = query.Where(p => p.AssignedDoctorUserId == currentUser.Id);
            }
            // Admin ziet alles – geen extra filter

            // 🔍 Filter op naam (voor- of achternaam bevat q)
            if (!string.IsNullOrWhiteSpace(q))
            {
                var needle = q.Trim();
                query = query.Where(p =>
                    p.FirstName.Contains(needle) ||
                    p.LastName.Contains(needle));
            }

            // 🔍 Filter op status (enum-naam, bv. "Active", "Completed", "OnHold", "IntakePlanned")
            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<PatientStatus>(status, ignoreCase: true, out var parsedStatus))
            {
                query = query.Where(p => p.Status == parsedStatus);
            }

            // simpele sortering; je kunt 'sort' nog gebruiken als je wilt
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
        /// Dokter mag alleen eigen patiënt zien, Admin alles.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<object>> GetPatient(int id)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var query = _context.Patients.AsNoTracking().Where(p => p.Id == id);

            if (currentUser.Role == "Doctor")
            {
                query = query.Where(p => p.AssignedDoctorUserId == currentUser.Id);
            }

            var p = await query.FirstOrDefaultAsync();
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
        /// Zelfde beveiliging: doctor alleen eigen patiënt.
        /// </summary>
        [HttpGet("{id:int}/dossier")]
        public async Task<ActionResult<object>> GetPatientDossier(int id)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            try
            {
                var query = _context.Patients.AsNoTracking().Where(p => p.Id == id);

                if (currentUser.Role == "Doctor")
                {
                    query = query.Where(p => p.AssignedDoctorUserId == currentUser.Id);
                }

                var patient = await query.FirstOrDefaultAsync();

                if (patient == null)
                    return NotFound(new { message = $"Patient {id} not found or not accessible." });

                var dossier = new
                {
                    patient.Id,
                    patient.FirstName,
                    patient.LastName,
                    patient.Status,
                    patient.StartDate,
                    patient.DateOfBirth,
                    patient.Diagnosis,
                    patient.Notes,
                    patient.AssignedDoctorUserId
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