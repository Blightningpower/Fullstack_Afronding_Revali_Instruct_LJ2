using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RevaliInstruct.Core.Data;
using RevaliInstruct.Core.Entities;

namespace RevaliInstruct.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public PatientsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? q, [FromQuery] string? status,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? sort = null)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _db.Patients.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var qLower = q.Trim().ToLower();
                query = query.Where(p => p.FirstName.ToLower().Contains(qLower) || p.LastName.ToLower().Contains(qLower));
            }

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<PatientStatus>(status, true, out var st))
            {
                query = query.Where(p => p.Status == st);
            }
            else if (!string.IsNullOrWhiteSpace(status))
            {
                return BadRequest("Unknown status value");
            }

            query = sort switch
            {
                "lastname" => query.OrderBy(p => p.LastName),
                "startDate_desc" => query.OrderByDescending(p => p.StartDate),
                _ => query.OrderBy(p => p.LastName)
            };

            var total = await query.CountAsync();
            Response.Headers["X-Total-Count"] = total.ToString();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    p.Id,
                    p.FirstName,
                    p.LastName,
                    StartDate = p.StartDate.ToString("yyyy-MM-dd"),
                    Status = p.Status.ToString()
                })
                .ToListAsync();

            return Ok(new { data = items, page, pageSize });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var p = await _db.Patients.AsNoTracking()
                        .Where(x => x.Id == id)
                        .Select(p => new
                        {
                            p.Id,
                            p.FirstName,
                            p.LastName,
                            StartDate = p.StartDate.ToString("yyyy-MM-dd"),
                            Status = p.Status.ToString(),
                            p.Notes // of andere velden die je wil tonen
                        })
                        .SingleOrDefaultAsync();

            if (p == null) return NotFound();
            return Ok(p);
        }

    }
}