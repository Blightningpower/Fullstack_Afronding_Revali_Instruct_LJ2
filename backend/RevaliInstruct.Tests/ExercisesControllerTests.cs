using Microsoft.EntityFrameworkCore;
using RevaliInstruct.Api.Controllers;
using RevaliInstruct.Core.Data;
using RevaliInstruct.Core.Entities;
using Xunit;
using FluentAssertions;

namespace RevaliInstruct.Tests
{
    public class ExercisesControllerTests
    {
        [Fact]
        public async Task GetLibrary_ReturnsAllAvailableExercises()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "ExercisesTestDb")
                .Options;

            using var context = new ApplicationDbContext(options);
            
            context.Exercises.AddRange(new List<Exercise>
            {
                new Exercise { Id = 1, Name = "Kniebuiging", Description = "Test 1" },
                new Exercise { Id = 2, Name = "Plank", Description = "Test 2" }
            });
            await context.SaveChangesAsync();

            var controller = new ExercisesController(context);

            // Act
            var result = await controller.GetLibrary();

            // Assert
            result.Value.Should().NotBeNull();
            result.Value.Should().HaveCount(2);
            result.Value.Should().Contain(e => e.Name == "Kniebuiging");
        }
    }
}