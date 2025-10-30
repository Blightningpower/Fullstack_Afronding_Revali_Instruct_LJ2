using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RevaliInstruct.Api.Data;
using RevaliInstruct.Api.Models;

namespace RevaliInstruct.Api.Controllers
{
    [ApiController]
    [Route("auth")]
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
        public IActionResult Login([FromBody] AuthRequest req)
        {
            var user = _ctx.Users.SingleOrDefault(u => u.Username == req.Username);
            if (user == null) return Unauthorized(new { message = "Gebruiker niet gevonden" });

            bool ok = BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash);
            if (!ok) return Unauthorized(new { message = "Onjuist wachtwoord" });

            var secret = _config["Jwt:Secret"] ?? throw new InvalidOperationException("Jwt Secret ontbreekt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(token);

            var resp = new AuthResponse
            {
                Token = tokenString,
                Username = user.Username,
                Role = user.Role,
                DisplayName = user.DisplayName
            };

            return Ok(resp);
        }
    }
}