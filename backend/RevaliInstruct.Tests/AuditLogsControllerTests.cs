using Microsoft.EntityFrameworkCore;
using RevaliInstruct.Api.Controllers;
using RevaliInstruct.Core.Data;
using RevaliInstruct.Core.Entities;
using Xunit;
using FluentAssertions;

namespace RevaliInstruct.Tests
{
    public class AuditLogsControllerTests
    {
        [Fact]
        public async Task GetLogs_ReturnsLogsOrderedByTimestampDescending()
        {
            // Arrange: Setup
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "AuditLogsTestDb")
                .Options;

            using var context = new ApplicationDbContext(options);

            // Voeg logs toe met verschillende tijden
            var oldLog = new AuditLog { Id = 1, Action = "OLD", Timestamp = DateTime.Now.AddDays(-1), Details = "Oud", TableName = "Tests" };
            var newLog = new AuditLog { Id = 2, Action = "NEW", Timestamp = DateTime.Now, Details = "Nieuw", TableName = "Tests" };
            
            context.AuditLogs.AddRange(oldLog, newLog);
            await context.SaveChangesAsync();

            var controller = new AuditLogsController(context);

            // Act
            var result = await controller.GetLogs();

            // Assert: Controleer of de nieuwste log bovenaan staat
            result.Value.Should().NotBeNull();
            result.Value!.First().Action.Should().Be("NEW");
            result.Value!.Last().Action.Should().Be("OLD");
        }
    }
}