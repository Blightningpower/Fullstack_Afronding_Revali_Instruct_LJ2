using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using RevaliInstruct.Core.Data;
using RevaliInstruct.Core.Entities;
using RevaliInstruct.Api.Dtos;

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
        public async Task<IActionResult> Login([FromBody] LoginRequestDto req)
        {
            if (req == null)
                return BadRequest(new { error = "username and password required" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1. User ophalen via EF
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Username == req.Username);

            if (user == null)
                return Unauthorized(new { error = "invalid credentials" });

            if (string.IsNullOrEmpty(req.Password) || string.IsNullOrEmpty(user.PasswordHash))
                return Unauthorized(new { error = "invalid credentials" });

            // 2. Wachtwoord checken (BCrypt)
            if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            {
                // Fallback voor ongehashte wachtwoorden in dev-omgeving
                if (user.PasswordHash == req.Password)
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password);
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return Unauthorized(new { error = "invalid credentials" });
                }
            }

            // 3. JWT token genereren
            var (token, expires, error) = GenerateJwtToken(user);
            if (token == null)
            {
                // Als dit faalt, zie je een 500-error in de browser
                return StatusCode(500, new { error = error ?? "failed to generate token" });
            }

            return Ok(new { token, expiresUtc = expires });
        }

        private (string? token, DateTime expiresUtc, string? error) GenerateJwtToken(User user)
        {
            // PRIORITEIT: Gebruik de sleutel die Program.cs uit de .env heeft geladen
            var key = _config["JWT_SECRET"] 
                      ?? _config["Jwt:Key"]
                      ?? _config["Jwt:Secret"]
                      ?? Environment.GetEnvironmentVariable("JWT_SECRET");

            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];

            if (string.IsNullOrEmpty(key))
            {
                return (null, default, "JWT key is not configured in backend settings.");
            }

            // Bij een lege key klapt deze regel eruit met een 500 error
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddHours(1);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username ?? string.Empty),
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
}