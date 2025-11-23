using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RevaliInstruct.Core.Data;
using RevaliInstruct.Core.Entities;

namespace RevaliInstruct.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PatientsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public PatientsController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Haalt een lijst van patiënten op.
        /// Ondersteunt zoeken op naam (q) en filteren op status.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPatients(
            [FromQuery] string? q,
            [FromQuery] string? status,
            CancellationToken ct = default)
        {
            var query = _db.Patients.AsNoTracking();

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

            var rows = await query
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .Select(p => new
                {
                    p.Id,
                    p.FirstName,
                    p.LastName,
                    // Laat datum weg als het de default MinValue is
                    DateOfBirth = p.DateOfBirth == DateTime.MinValue ? (DateTime?)null : p.DateOfBirth,
                    StartDate   = p.StartDate   == DateTime.MinValue ? (DateTime?)null : p.StartDate,
                    Status      = p.Status.ToString(),
                    p.Notes
                })
                .ToListAsync(ct);

            return Ok(rows);
        }

        /// <summary>
        /// Haalt de details op van één patiënt.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPatient(int id, CancellationToken ct = default)
        {
            var patient = await _db.Patients
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.DateOfBirth,
                    x.StartDate,
                    Status = x.Status.ToString(),
                    x.Notes
                })
                .FirstOrDefaultAsync(ct);

            if (patient == null)
                return NotFound();

            return Ok(patient);
        }
    }
}