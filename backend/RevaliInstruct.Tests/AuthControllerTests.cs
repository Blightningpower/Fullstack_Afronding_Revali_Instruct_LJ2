using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using RevaliInstruct.Api.Controllers;
using RevaliInstruct.Api.Dtos;
using RevaliInstruct.Core.Data;
using RevaliInstruct.Core.Entities;
using Microsoft.Extensions.Configuration;
using Xunit;
using FluentAssertions;

namespace RevaliInstruct.Tests
{
    public class AuthControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _context = new ApplicationDbContext(options);

            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.Setup(c => c["JWT_SECRET"]).Returns("SuperGeheimeSleutelVanMinimaal32Tekens!");

            _controller = new AuthController(_context, _mockConfig.Object);
        }

        [Fact]
        public async Task Login_CorrectCredentials_ReturnsToken()
        {
            // Arrange
            var password = "password123";
            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            _context.Users.Add(new User { Username = "testuser", PasswordHash = hash, Role = "Revalidatiearts" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Login(new LoginRequestDto { Username = "testuser", Password = password });

            // Assert: Fix voor CS8602 door expliciete type check en null-safety
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().NotBeNull();
            okResult.Value!.ToString().Should().Contain("token");
        }
    }
}