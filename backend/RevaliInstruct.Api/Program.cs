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

builder.Services.AddControllers();
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
            ClockSkew = TimeSpan.FromSeconds(30)
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

app.UseSwagger();
app.UseSwaggerUI(o =>
{
    o.SwaggerEndpoint("/swagger/v1/swagger.json", "RevaliInstruct.Api v1");
    o.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
});

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(MyAllowedOrigins);
app.UseAuthentication();

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

app.UseAuthorization();

// Voeg dossier endpoint toe (US2: Patiëntendossier Inzien)
// 1 handler, 2 routes (/api/... en legacy zonder prefix)
async Task<IResult> DossierHandler(int id, ApplicationDbContext db, CancellationToken ct)
{
    var p = await db.Patients.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    if (p is null) return Results.NotFound(new { message = "Patient not found" });

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

    // Demo/fallback invulling voor contactgegevens
    var r = new Random(id);
    string safeLast = (patient.LastName ?? "").Replace(" ", "").ToLowerInvariant();
    if (string.IsNullOrWhiteSpace(patient.Email) && !string.IsNullOrWhiteSpace(patient.FirstName) && !string.IsNullOrWhiteSpace(patient.LastName))
        patient.Email = $"{char.ToLowerInvariant(patient.FirstName![0])}.{safeLast}@examplehospital.nl";
    if (string.IsNullOrWhiteSpace(patient.Phone))
        patient.Phone = $"06-{r.Next(10000000, 99999999)}";
    if (string.IsNullOrWhiteSpace(patient.ReferringDoctor))
        patient.ReferringDoctor = $"Huisarts {Convert.ToChar('A' + r.Next(0, 26))}";

    // Exercises
    var exerciseNames = new[] { "Kniebuiging", "Schouder abductie", "Heuplift", "Enkel cirkels", "Quadriceps stretch", "Hamstring stretch", "Core stabiliteit" };
    var exercises = Enumerable.Range(1, r.Next(3, 6))
        .Select(i => new ExerciseDto
        {
            Id = i,
            Name = exerciseNames[r.Next(exerciseNames.Length)],
            Status = r.NextDouble() < 0.5 ? "Toegewezen" : "Afgevinkt"
        })
        .ToArray();

    // Pain timeline (laatste 14 dagen)
    var pains = Enumerable.Range(0, 14)
        .Select(d => new PainEntryDto
        {
            Date = DateTime.UtcNow.Date.AddDays(-d),
            Level = r.Next(1, 10),
            Note = r.NextDouble() < 0.25 ? "Na oefening licht toegenomen" : null
        })
        .OrderBy(x => x.Date)
        .ToArray();

    // Activities (laatste week)
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

    // Medications
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

    // Accessories
    var accCatalog = new[] { "Elastische band (medium)", "Kniebrace", "Enkelbandage", "Schouderband" };
    var accessories = Enumerable.Range(0, r.Next(0, 3))
        .Select(_ => new AccessoryDto
        {
            Name = accCatalog[r.Next(accCatalog.Length)],
            Instructions = "Gebruik tijdens oefening of activiteit"
        })
        .ToArray();

    // Appointments (2 verleden, 1 toekomst)
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

app.MapControllers();
app.Urls.Add("http://localhost:5000");
await app.RunAsync();

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
    public string? Status { get; set; } // bv. 'Toegewezen', 'Afgevinkt'
}

internal sealed class PainEntryDto
{
    public DateTime Date { get; set; }
    public int Level { get; set; } // 0-10
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
    public string? Type { get; set; } // bv. intake, controle
    public string? Status { get; set; } // gepland/afgerond
}
#endregion