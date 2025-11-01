using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using RevaliInstruct.Core.Data;
using RevaliInstruct.Api.Models;

namespace RevaliInstruct.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _ctx;
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext ctx, IConfiguration config)
        {
            _ctx = ctx;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest(new { message = "Username en wachtwoord zijn verplicht" });

            // async DB-query
            var user = await _ctx.Users.SingleOrDefaultAsync(u => u.Username == req.Username);

            if (user == null)
                return Unauthorized(new { message = "Gebruiker niet gevonden" });

            // bcrypt verify (zorg dat je BCrypt.Net-Next package hebt)
            bool ok = BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash);
            if (!ok) return Unauthorized(new { message = "Onjuist wachtwoord" });

            // JWT secret ophalen (kijk in appsettings of env vars)
            var secret = _config["Jwt:Secret"] ?? _config["JWT__Secret"];
            if (string.IsNullOrEmpty(secret))
                throw new InvalidOperationException("Jwt Secret ontbreekt (configureer Jwt:Secret of env JWT__Secret)");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role ?? string.Empty)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                token = tokenString,
                user = new
                {
                    id = user.UserId,
                    username = user.Username,
                    displayName = user.DisplayName,
                    role = user.Role
                }
            });
        }
    }
}