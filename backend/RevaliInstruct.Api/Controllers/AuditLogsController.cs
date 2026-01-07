using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RevaliInstruct.Core.Data;
using RevaliInstruct.Core.Entities;
using Microsoft.AspNetCore.Authorization;

namespace RevaliInstruct.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AuditLogsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuditLogsController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditLog>>> GetLogs()
        {
            return await _context.AuditLogs
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();
        }
    }
}