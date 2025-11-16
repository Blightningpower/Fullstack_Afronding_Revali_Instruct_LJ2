using System;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RevaliInstruct.Core.Data;
using RevaliInstruct.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace RevaliInstruct.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            if (req == null)
                return BadRequest(new { error = "username and password required" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // --- replaced EF lookup with safe direct DB query to avoid schema-mismatch errors ---
            User? user = null;
            var conn = _context.Database.GetDbConnection();
            try
            {
                if (conn.State != System.Data.ConnectionState.Open)
                    await conn.OpenAsync();

                // Stap 1: haal basisvelden op (Id, Username, PasswordHash, Role)
                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"SELECT TOP(1) [Id], [Username], [PasswordHash], [Role]
                                    FROM dbo.Users
                                    WHERE [Username] = @u";
                var p = cmd.CreateParameter();
                p.ParameterName = "@u";
                p.Value = req.Username ?? (object)DBNull.Value;
                cmd.Parameters.Add(p);

                int id;
                string? username;
                string? passwordHash;
                string? role;

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return Unauthorized(new { error = "invalid credentials" });

                    id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                    username = reader.IsDBNull(1) ? null : reader.GetString(1);
                    passwordHash = reader.IsDBNull(2) ? null : reader.GetString(2);
                    role = reader.IsDBNull(3) ? null : reader.GetString(3);
                }

                // Stap 2 (optioneel): controleer of kolom DisplayName bestaat en lees deze indien aanwezig
                string? displayName = null;
                using (var colCmd = conn.CreateCommand())
                {
                    colCmd.CommandText = @"
SELECT CASE WHEN EXISTS(
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.Users') AND name = @colName
) THEN 1 ELSE 0 END";
                    var cp = colCmd.CreateParameter();
                    cp.ParameterName = "@colName";
                    cp.Value = "DisplayName";
                    colCmd.Parameters.Add(cp);

                    var exists = (int?)await colCmd.ExecuteScalarAsync();
                    if (exists == 1)
                    {
                        using var dnCmd = conn.CreateCommand();
                        dnCmd.CommandText = "SELECT TOP(1) [DisplayName] FROM dbo.Users WHERE [Id] = @id";
                        var ip = dnCmd.CreateParameter();
                        ip.ParameterName = "@id";
                        ip.Value = id;
                        dnCmd.Parameters.Add(ip);

                        var val = await dnCmd.ExecuteScalarAsync();
                        if (val != null && val != DBNull.Value) displayName = val.ToString();
                    }
                }

                user = new User
                {
                    Id = id,
                    Username = username ?? req.Username!,
                    PasswordHash = passwordHash ?? string.Empty,
                    Role = role ?? string.Empty,
                    FullName = displayName
                };
            }
            finally
            {
                try { await conn.CloseAsync(); } catch { }
            }
            // --- end replaced lookup ---

            if (string.IsNullOrEmpty(req.Password) || string.IsNullOrEmpty(user.PasswordHash))
                return Unauthorized(new { error = "invalid credentials" });

            // Probeer BCrypt eerst
            if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            {
                // Fallback: plaintext -> convert (dev convenience)
                if (user.PasswordHash == req.Password)
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password);
                    try
                    {
                        _context.Users.Update(user);
                        await _context.SaveChangesAsync();
                    }
                    catch
                    {
                        // ignore update errors (schema may not match)
                    }
                }
                else
                {
                    return Unauthorized(new { error = "invalid credentials" });
                }
            }

            var (token, expires, error) = GenerateJwtToken(user);
            if (token == null)
            {
                return StatusCode(500, new { error = error ?? "failed to generate token" });
            }

            return Ok(new { token, expiresUtc = expires });
        }

        private (string? token, DateTime expiresUtc, string? error) GenerateJwtToken(User user)
        {
            // probeer meerdere mogelijke configuratie-keys/env-vars als fallback
            var key = _config["Jwt:Key"]
                      ?? _config["Jwt:Secret"]
                      ?? _config["JWT__Secret"]
                      ?? _config["JWT:Secret"]
                      ?? Environment.GetEnvironmentVariable("JWT_SECRET")
                      ?? Environment.GetEnvironmentVariable("JWT__Secret");

            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];

            if (string.IsNullOrEmpty(key))
            {
                return (null, default, "JWT key is not configured (Jwt:Key / Jwt:Secret / JWT_SECRET)");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddHours(1);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName ?? user.Username ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new JwtSecurityToken(
                issuer: string.IsNullOrEmpty(issuer) ? null : issuer,
                audience: string.IsNullOrEmpty(audience) ? null : audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
            return (tokenString, expires, null);
        }
    }

    public class LoginRequest
    {
        // Mark properties nullable to avoid CS8618 and add Required for validation
        [System.ComponentModel.DataAnnotations.Required]
        public string? Username { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string? Password { get; set; }
    }
}