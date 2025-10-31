// Program.cs
using Microsoft.EntityFrameworkCore;
using RevaliInstruct.Api.Data;   // <- zorgt dat ApplicationDbContext gevonden wordt
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

// Swagger/OpenAPI (optioneel maar handig in Development)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext: gebruik connection string uit appsettings.json
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrEmpty(conn))
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseMySql(conn, ServerVersion.AutoDetect(conn))
    );
}
else
{
    // fallback / logmelding (optioneel)
    Console.WriteLine("Geen DefaultConnection gevonden in appsettings.json");
}

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Als je authentication toevoegt, zet UseAuthentication() vóór UseAuthorization()
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();