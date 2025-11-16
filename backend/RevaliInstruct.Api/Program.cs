using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RevaliInstruct.Core.Data;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
var MyAllowedOrigins = "_myAllowedOrigins";

// Handmatige .env ondersteuning 
var envFile = Path.Combine(Directory.GetCurrentDirectory(), ".env");
if (File.Exists(envFile))
{
    foreach (var line in File.ReadAllLines(envFile))
    {
        if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#")) continue;
        var parts = line.Split('=', 2);
        if (parts.Length == 2)
        {
            Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
        }
    }
}

// Add services to the container.
builder.Services.AddControllers(); // <- zorg dat controllers geregistreerd zijn
builder.Services.AddEndpointsApiExplorer();

// Swagger met JWT Bearer support
builder.Services.AddSwaggerGen(c =>
{
    var xml = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xml);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RevaliInstruct.Api", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using Bearer scheme"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// CORS policy voor Vite dev server
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowedOrigins, policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configuratiebronnen laden (json + env vars na .env parsing)
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// JWT authenticatie configuratie (na config load zodat .env/ENV werken)
var secret = builder.Configuration["Jwt:Secret"] ?? builder.Configuration["JWT__Secret"];
var authBuilder = builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
});

if (!string.IsNullOrEmpty(secret))
{
    authBuilder.AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ClockSkew = TimeSpan.FromSeconds(30),

            // Zorg dat role/name claims correct gemapt worden
            NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier,
            RoleClaimType = System.Security.Claims.ClaimTypes.Role
        };

        // Logging / debugging hooks zodat je ziet waarom authenticatie faalt
        options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                try
                {
                    // controleer zowel 'Authorization' als lowercase 'authorization'
                    var hdr = ctx.Request.Headers["Authorization"].FirstOrDefault()
                              ?? ctx.Request.Headers["authorization"].FirstOrDefault();

                    // ook probeer access_token query fallback (bijv. signalr / testing)
                    if (string.IsNullOrWhiteSpace(hdr))
                    {
                        var q = ctx.Request.Query["access_token"].FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(q))
                        {
                            Console.WriteLine("JwtBearer OnMessageReceived: found token in query string");
                            ctx.Token = q;
                            return Task.CompletedTask;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"JwtBearer OnMessageReceived Authorization header: {hdr}");
                        if (hdr.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        {
                            ctx.Token = hdr.Substring("Bearer ".Length).Trim();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("OnMessageReceived error: " + ex.Message);
                }
                return Task.CompletedTask;
            },
            OnTokenValidated = ctx =>
            {
                var id = ctx.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var role = ctx.Principal?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                Console.WriteLine($"JwtBearer token validated. NameId={id ?? "<null>"} Role={role ?? "<null>"}");
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = ctx =>
            {
                Console.WriteLine("JwtBearer authentication failed: " + ctx.Exception?.Message);
                return Task.CompletedTask;
            }
        };
    });
}
else
{
    Console.WriteLine("JWT secret is leeg — JWT validatie wordt overgeslagen in deze sessie.");
}

// Database connectiestring samenstellen (fallback naar environment variabelen)
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
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
               .EnableDetailedErrors()
               .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
    );
}
else
{
    Console.WriteLine("Geen DefaultConnection gevonden");
}

var app = builder.Build();

// Optional automatic reset on startup (set env var RESET_DB=1)
if (Environment.GetEnvironmentVariable("RESET_DB") == "1")
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DbInitializer.ResetAndSeedAsync(db);
    Console.WriteLine("Database reset (RESET_DB=1).");
}

// START: ensure seeding runs automatically in Development so deleted users get re-seeded
if (app.Environment.IsDevelopment())
{
    try
    {
        using var seedScope = app.Services.CreateScope();
        var db = seedScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await DbInitializer.SeedAsync(db);
        Console.WriteLine("DbInitializer.SeedAsync executed at startup (development).");
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Automatic seeding on startup failed.");
    }
}
// END: automatic dev seeding

// Exception handling: gedetailleerd in Development, generiek in Production
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler(errApp =>
    {
        errApp.Run(async ctx =>
        {
            var feature = ctx.Features.Get<IExceptionHandlerFeature>();
            var ex = feature?.Error;
            var logger = ctx.RequestServices.GetRequiredService<ILogger<Program>>();
            if (ex != null) logger.LogError(ex, "Unhandled exception");
            ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
            ctx.Response.ContentType = "application/json";
            await ctx.Response.WriteAsJsonAsync(new { message = "Unexpected server error" });
        });
    });
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Zorg voor juiste volgorde van middleware
app.UseRouting();

// <-- voeg CORS middleware toe vóór authentication
app.UseCors(MyAllowedOrigins);

app.UseAuthentication();
app.UseAuthorization();

// Map attribute routed controllers
app.MapControllers();

// Zet SESSION_CONTEXT voor Row-Level Security (RLS)
// Haalt UserId en IsAdmin uit JWT claims en schrijft naar SQL Server session
app.Use(async (ctx, next) =>
{
    string? FindClaim(params string[] types) =>
        types.Select(t => ctx.User.FindFirst(t)?.Value).FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));

    int userId = 0;
    var uidStr = FindClaim(ClaimTypes.NameIdentifier, "sub", "uid", "UserId", "user_id");
    if (!string.IsNullOrWhiteSpace(uidStr)) int.TryParse(uidStr, out userId);

    bool isAdmin = ctx.User.IsInRole("Admin") ||
        (FindClaim(ClaimTypes.Role, "role", "roles")?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Any(r => string.Equals(r, "Admin", StringComparison.OrdinalIgnoreCase)) ?? false);

    try
    {
        var db = ctx.RequestServices.GetRequiredService<ApplicationDbContext>();
        await db.Database.ExecuteSqlRawAsync("EXEC sys.sp_set_session_context @key=N'UserId', @value={0};", userId);
        await db.Database.ExecuteSqlRawAsync("EXEC sys.sp_set_session_context @key=N'IsAdmin', @value={0};", isAdmin ? "true" : "false");
    }
    catch (Exception ex)
    {
        var logger = ctx.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "SESSION_CONTEXT error");
    }

    await next();
});

// Voeg dossier endpoint toe (US2: Patiëntendossier Inzien)
// 1 handler, 2 routes (/api/... en legacy zonder prefix)
async Task<IResult> DossierHandler(int id, ApplicationDbContext db, HttpContext ctx, CancellationToken ct)
{
    // Haal UserId en Role uit JWT claims
    var userIdClaim = ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? ctx.User.FindFirst("sub")?.Value;

    if (!int.TryParse(userIdClaim, out var userId))
        return Results.Unauthorized();

    var isAdmin = ctx.User.IsInRole("Admin");
    var isDoctor = isAdmin || ctx.User.IsInRole("Doctor");
    if (!isDoctor) return Results.Forbid();

    var p = await db.Patients.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    if (p is null) return Results.NotFound(new { message = "Patient not found" });

    if (!isAdmin)
    {
        var assignedDocId = EF.Property<int?>(p, "AssignedDoctorUserId");
        if (assignedDocId != userId) return Results.Forbid();
    }

    var patient = new BasicPatientDto
    {
        Id = p.Id,
        FirstName = p.FirstName,
        LastName = p.LastName,
        Email = null,
        Phone = null,
        ReferringDoctor = null,
        DateOfBirth = p.DateOfBirth == DateTime.MinValue ? null : p.DateOfBirth,
        StartDate = p.StartDate == DateTime.MinValue ? null : p.StartDate,
        Status = p.Status.ToString()
    };

    var r = new Random(id);
    string safeLast = (patient.LastName ?? "").Replace(" ", "").ToLowerInvariant();
    if (string.IsNullOrWhiteSpace(patient.Email) && !string.IsNullOrWhiteSpace(patient.FirstName) && !string.IsNullOrWhiteSpace(patient.LastName))
        patient.Email = $"{char.ToLowerInvariant(patient.FirstName![0])}.{safeLast}@examplehospital.nl";
    if (string.IsNullOrWhiteSpace(patient.Phone))
        patient.Phone = $"06-{r.Next(10000000, 99999999)}";
    if (string.IsNullOrWhiteSpace(patient.ReferringDoctor))
        patient.ReferringDoctor = $"Huisarts {Convert.ToChar('A' + r.Next(0, 26))}";

    var exerciseNames = new[] { "Kniebuiging", "Schouder abductie", "Heuplift", "Enkel cirkels", "Quadriceps stretch", "Hamstring stretch", "Core stabiliteit" };
    var exercises = Enumerable.Range(1, r.Next(3, 6))
        .Select(i => new ExerciseDto
        {
            Id = i,
            Name = exerciseNames[r.Next(exerciseNames.Length)],
            Status = r.NextDouble() < 0.5 ? "Toegewezen" : "Afgevinkt"
        })
        .ToArray();

    var pains = Enumerable.Range(0, 14)
        .Select(d => new PainEntryDto
        {
            Date = DateTime.UtcNow.Date.AddDays(-d),
            Level = r.Next(1, 10),
            Note = r.NextDouble() < 0.25 ? "Na oefening licht toegenomen" : null
        })
        .OrderBy(x => x.Date)
        .ToArray();

    var activityNames = new[] { "Wandelen", "Fietsen", "Huishoudelijk werk", "Zwemmen", "Krachttraining" };
    var activities = Enumerable.Range(0, r.Next(2, 5))
        .Select(i => new ActivityLogDto
        {
            Date = DateTime.UtcNow.Date.AddDays(-r.Next(0, 7)),
            Activity = activityNames[r.Next(activityNames.Length)],
            Notes = r.NextDouble() < 0.3 ? "Ging goed" : null
        })
        .OrderByDescending(x => x.Date)
        .ToArray();

    var medCatalog = new (string name, string dose)[] {
        ("Paracetamol", "500 mg 1-3x/dag"),
        ("Ibuprofen", "400 mg 1-2x/dag"),
        ("Naproxen", "250 mg 2x/dag")
    };
    var meds = Enumerable.Range(0, r.Next(0, 3))
        .Select(_ => {
            var m = medCatalog[r.Next(medCatalog.Length)];
            return new MedicationDto { Name = m.name, Dosage = m.dose };
        })
        .ToArray();

    var accCatalog = new[] { "Elastische band (medium)", "Kniebrace", "Enkelbandage", "Schouderband" };
    var accessories = Enumerable.Range(0, r.Next(0, 3))
        .Select(_ => new AccessoryDto
        {
            Name = accCatalog[r.Next(accCatalog.Length)],
            Instructions = "Gebruik tijdens oefening of activiteit"
        })
        .ToArray();

    var apptTypes = new[] { "Intake", "Controle", "Evaluatie" };
    var appointments = new[]
    {
        new AppointmentDto { Date = DateTime.UtcNow.AddDays(-21), Type = apptTypes[r.Next(apptTypes.Length)], Status = "Afgerond" },
        new AppointmentDto { Date = DateTime.UtcNow.AddDays(-7),  Type = apptTypes[r.Next(apptTypes.Length)], Status = "Afgerond" },
        new AppointmentDto { Date = DateTime.UtcNow.AddDays(7),   Type = apptTypes[r.Next(apptTypes.Length)], Status = "Gepland" }
    };

    var dossier = new DossierDto
    {
        Patient = patient,
        Exercises = exercises,
        PainTimeline = pains,
        Activities = activities,
        Medications = meds,
        Accessories = accessories,
        Appointments = appointments
    };

    return Results.Ok(dossier);
}

app.MapGet("/api/patients/{id:int}/dossier", DossierHandler);
app.MapGet("/patients/{id:int}/dossier", DossierHandler);
app.MapGet("/api/dossier/{id:int}", DossierHandler);

// Dev endpoint to reset and reseed manually
app.MapPost("/api/dev/reset-db", async (IServiceProvider sp) =>
{
    using var scope = sp.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DbInitializer.ResetAndSeedAsync(db);
    return Results.Ok(new { reset = true });
});

app.Run();

#region DTOs voor dossier (US2)
internal sealed class PatientsListItemDto
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? StartDate { get; set; }
    public string? Status { get; set; }
}

internal sealed class DossierDto
{
    public BasicPatientDto Patient { get; set; } = default!;
    public IEnumerable<ExerciseDto> Exercises { get; set; } = Enumerable.Empty<ExerciseDto>();
    public IEnumerable<PainEntryDto> PainTimeline { get; set; } = Enumerable.Empty<PainEntryDto>();
    public IEnumerable<ActivityLogDto> Activities { get; set; } = Enumerable.Empty<ActivityLogDto>();
    public IEnumerable<MedicationDto> Medications { get; set; } = Enumerable.Empty<MedicationDto>();
    public IEnumerable<AccessoryDto> Accessories { get; set; } = Enumerable.Empty<AccessoryDto>();
    public IEnumerable<AppointmentDto> Appointments { get; set; } = Enumerable.Empty<AppointmentDto>();
}

internal sealed class BasicPatientDto
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? ReferringDoctor { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime? StartDate { get; set; }
    public string? Status { get; set; }
}

internal sealed class ExerciseDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Status { get; set; }
}

internal sealed class PainEntryDto
{
    public DateTime Date { get; set; }
    public int Level { get; set; }
    public string? Note { get; set; }
}

internal sealed class ActivityLogDto
{
    public DateTime Date { get; set; }
    public string? Activity { get; set; }
    public string? Notes { get; set; }
}

internal sealed class MedicationDto
{
    public string? Name { get; set; }
    public string? Dosage { get; set; }
}

internal sealed class AccessoryDto
{
    public string? Name { get; set; }
    public string? Instructions { get; set; }
}

internal sealed class AppointmentDto
{
    public DateTime Date { get; set; }
    public string? Type { get; set; }
    public string? Status { get; set; }
}
#endregion