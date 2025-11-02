using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RevaliInstruct.Core.Data;
using System.Data;
using System.Data.Common;
using RevaliInstruct.Api.Models;
using RevaliInstruct.Core.Entities;
using CoreUser = RevaliInstruct.Core.Entities.User; // Add alias to disambiguate 'User'

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
        public async Task<IActionResult> GetPatients([FromQuery] string? q, [FromQuery] string? status, CancellationToken ct = default)
        {
            var query = _db.Patients.AsNoTracking();

            // Filter op naam (database-level)
            if (!string.IsNullOrWhiteSpace(q))
            {
                var needle = q.Trim();
                query = query.Where(p => 
                    p.FirstName.Contains(needle) || 
                    p.LastName.Contains(needle));
            }

            // Filter op status (enum -> enum vergelijking)
            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<PatientStatus>(status, true, out var parsedStatus))
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
                    DateOfBirth = p.DateOfBirth == DateTime.MinValue ? (DateTime?)null : p.DateOfBirth,
                    StartDate = p.StartDate == DateTime.MinValue ? (DateTime?)null : p.StartDate,
                    Status = p.Status.ToString(),
                    p.Notes
                })
                .ToListAsync(ct);

            return Ok(rows);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPatient(int id, CancellationToken ct = default)
        {
            // Lees Status als string om enum-conversieproblemen te voorkomen
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
                    StatusRaw = EF.Property<string>(x, "Status"),
                    x.Notes
                })
                .FirstOrDefaultAsync(ct);

            if (row == null) return NotFound();

            return Ok(new
            {
                row.Id,
                row.FirstName,
                row.LastName,
                row.DateOfBirth,
                row.StartDate,
                Status = row.StatusRaw,
                row.Notes
            });
        }

        // Seed extra patiënten via EF/ADO; handig alternatief voor SQL-scripts
        // Tip: roep aan met POST /api/patients/dev/seed-more
        [HttpPost("dev/seed-more")]
        [AllowAnonymous] // desgewenst weghalen als je auth wilt afdwingen
        public async Task<IActionResult> SeedMorePatients(CancellationToken ct = default)
        {
            var names = new (string First, string Last)[] {
                ("Anna","de Vries"), ("Bram","Jansen"), ("Eva","Bakker"), ("Lars","Visser"),
                ("Sanne","Smit"), ("Daan","Meijer"), ("Julia","Mulder"), ("Noah","Bos"),
                ("Lotte","Vos"), ("Milan","Peters"), ("Tess","Kok"), ("Finn","van Dijk"),
                ("Nina","Kuiper"), ("Thijs","van der Meer"), ("Sara","Willems"), ("Ruben","Hendriks"),
                ("Lisa","Schouten"), ("Gijs","Vermeulen"), ("Fleur","Post"), ("Jesse","Koster"),
                ("Roos","Maas"), ("Bo","Hoekstra")
            };

            // Bepaal kolom-aanwezigheid en nullability voor AssignedDoctorUserId
            var (hasAssigned, assignedNullable) = await CheckAssignedDoctorColumnAsync(ct);

            int? doctorId = null;
            if (hasAssigned)
            {
                // 1) Neem bestaande arts-id uit patiënten
                doctorId = await _db.Patients
                    .Where(p => EF.Property<int?>(p, "AssignedDoctorUserId") != null)
                    .Select(p => EF.Property<int?>(p, "AssignedDoctorUserId"))
                    .OrderBy(id => id)
                    .FirstOrDefaultAsync(ct);

                // 2) Zo niet, probeer Users tabel (als die bestaat)
                if (doctorId == null)
                {
                    var userEntity = _db.Model.FindEntityType(typeof(CoreUser));
                    if (userEntity != null)
                    {
                        var pkProp = userEntity.FindPrimaryKey()?.Properties.FirstOrDefault();
                        if (pkProp != null && (pkProp.ClrType == typeof(int) || pkProp.ClrType == typeof(int?)))
                        {
                            var keyName = pkProp.Name;
                            doctorId = await _db.Set<CoreUser>()
                                .OrderBy(u => EF.Property<int>(u, keyName))
                                .Select(u => (int?)EF.Property<int>(u, keyName))
                                .FirstOrDefaultAsync(ct);
                        }
                    }
                }

                // 3) Als nog steeds null en kolom niet nullable -> seed niet
                if (doctorId == null && !assignedNullable)
                {
                    return BadRequest(new { message = "Kan niet seeden: AssignedDoctorUserId is verplicht maar er is geen (geldige) user gevonden." });
                }
            }

            // Insert met raw SQL via EF (veilig en simpel voor string Status = 'Active')
            // 'Active' komt overeen met enum-naam PatientStatus.Active
            var rnd = new Random(1234);
            var inserted = 0;

            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            try
            {
                await using var conn = _db.Database.GetDbConnection();
                if (conn.State != ConnectionState.Open) await conn.OpenAsync(ct);

                foreach (var (first, last) in names)
                {
                    // skip als naam al bestaat
                    var exists = await _db.Patients.AsNoTracking()
                        .AnyAsync(p => p.FirstName == first && p.LastName == last, ct);
                    if (exists) continue;

                    var dob = DateTime.Parse("1965-01-01").AddDays(rnd.Next(0, 14610)); // ~40 jaar
                    var start = DateTime.Parse("2024-01-01").AddDays(rnd.Next(0, 730)); // 2024/2025
                    var status = "Active";

                    // Stel INSERT dynamisch samen op basis van AssignedDoctorUserId-kolom
                    string sql;
                    var cmd = conn.CreateCommand();
                    cmd.Transaction = (DbTransaction?)tx.GetDbTransaction();

                    if (hasAssigned)
                    {
                        sql = "INSERT INTO dbo.Patients (FirstName, LastName, DateOfBirth, StartDate, Status, AssignedDoctorUserId) VALUES (@first, @last, @dob, @start, @status, @doc)";
                        AddParam(cmd, "@doc", doctorId ?? (object)DBNull.Value);
                    }
                    else
                    {
                        sql = "INSERT INTO dbo.Patients (FirstName, LastName, DateOfBirth, StartDate, Status) VALUES (@first, @last, @dob, @start, @status)";
                    }

                    cmd.CommandText = sql;
                    AddParam(cmd, "@first", first);
                    AddParam(cmd, "@last", last);
                    AddParam(cmd, "@dob", dob);
                    AddParam(cmd, "@start", start);
                    AddParam(cmd, "@status", status);

                    inserted += await cmd.ExecuteNonQueryAsync(ct);
                }

                await tx.CommitAsync(ct);
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }

            return Ok(new { inserted });
        }

        private static void AddParam(DbCommand cmd, string name, object? value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(p);
        }

        private async Task<(bool hasAssigned, bool assignedNullable)> CheckAssignedDoctorColumnAsync(CancellationToken ct)
        {
            await using var conn = _db.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync(ct);
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COL_LENGTH('dbo.Patients','AssignedDoctorUserId') AS L";
            var has = (await cmd.ExecuteScalarAsync(ct)) is not null;

            var nullable = true;
            if (has)
            {
                await using var cmd2 = conn.CreateCommand();
                cmd2.CommandText = @"SELECT IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS 
                                     WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='Patients' AND COLUMN_NAME='AssignedDoctorUserId'";
                var v = (await cmd2.ExecuteScalarAsync(ct))?.ToString();
                nullable = string.Equals(v, "YES", StringComparison.OrdinalIgnoreCase);
            }
            return (has, nullable);
        }
    }
}