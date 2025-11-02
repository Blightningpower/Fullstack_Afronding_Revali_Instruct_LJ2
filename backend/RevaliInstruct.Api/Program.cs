using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RevaliInstruct.Core.Data;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
var MyAllowedOrigins = "_myAllowedOrigins";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowedOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // Vite dev server
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var secret = builder.Configuration["Jwt:Secret"] ?? builder.Configuration["JWT__Secret"];
if (!string.IsNullOrEmpty(secret))
{
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // dev only
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });
}

var conn = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

if (string.IsNullOrEmpty(conn))
{
    var host = Environment.GetEnvironmentVariable("MSSQL_HOST") ?? "localhost";
    var port = Environment.GetEnvironmentVariable("MSSQL_PORT") ?? "1433";
    var db = Environment.GetEnvironmentVariable("MSSQL_DATABASE") ?? "revali_db";
    var user = Environment.GetEnvironmentVariable("MSSQL_USER") ?? "revali_login";
    var pwd = Environment.GetEnvironmentVariable("MSSQL_PASSWORD") ?? "";

    conn = $"Server={host},{port};Database={db};User Id={user};Password={pwd};TrustServerCertificate=True;MultipleActiveResultSets=true;";
}

Console.WriteLine($"DEBUG: DefaultConnection = {(string.IsNullOrEmpty(conn) ? "<empty>" : conn)}");

if (!string.IsNullOrEmpty(conn))
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(conn, sqloptions => sqloptions.MigrationsAssembly("RevaliInstruct.Core"))
);
}
else
{
    Console.WriteLine("Geen DefaultConnection gevonden in appsettings.json");
}

var app = builder.Build();

// Wait-and-retry for DB, then migrate + seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var db = services.GetRequiredService<ApplicationDbContext>();

    var maxRetries = 10;
    var delay = TimeSpan.FromSeconds(5);

    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            logger.LogInformation("Applying migrations (attempt {Attempt}/{Max})...", attempt, maxRetries);
            db.Database.Migrate();

            logger.LogInformation("Seeding database...");
            await DbInitializer.SeedAsync(db);
            logger.LogInformation("Seeding finished.");
            break;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Database not ready yet (attempt {Attempt}/{Max}). Waiting {Delay}s...", attempt, maxRetries, delay.TotalSeconds);
            if (attempt == maxRetries) throw; // rethrow last exception
            await Task.Delay(delay);
            delay = delay + delay; // exponential-ish backoff
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// --- Run migrations + seed bij opstart (veilig in try/catch met logging)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        // run migrations first
        db.Database.Migrate();

        // seed
        await DbInitializer.SeedAsync(db);

        logger.LogInformation("Migrations and seeding completed.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
        throw;
    }
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.UseCors(MyAllowedOrigins);
app.Urls.Add("http://+:80");

// gebruik RunAsync zodat top-level await werkt
await app.RunAsync();