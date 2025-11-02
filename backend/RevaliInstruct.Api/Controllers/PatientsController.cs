using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RevaliInstruct.Api.Models;
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
        public PatientsController(ApplicationDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientListDto>>> GetPatients([FromQuery] string? q, [FromQuery] string? status)
        {
            var query = _db.Patients.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                query = query.Where(p => p.FirstName.Contains(term) || p.LastName.Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<PatientStatus>(status, ignoreCase: true, out var st))
            {
                query = query.Where(p => p.Status == st);
            }

            var rows = await query
                .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
                .Select(p => new
                {
                    p.Id,
                    p.FirstName,
                    p.LastName,
                    p.StartDate,
                    p.Status
                })
                .ToListAsync();

            var items = rows.Select(p => new PatientListDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                StartDate = p.StartDate,
                Status = p.Status.ToString()
            }).ToList();

            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPatient(int id)
        {
            var row = await _db.Patients
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.DateOfBirth,
                    x.StartDate,
                    x.Status,
                    x.Notes
                })
                .FirstOrDefaultAsync();

            if (row == null) return NotFound();

            return Ok(new
            {
                row.Id,
                row.FirstName,
                row.LastName,
                row.DateOfBirth,
                row.StartDate,
                Status = row.Status.ToString(),
                row.Notes
            });
        }
    }
}