using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RevaliInstruct.Api.Middleware;
using RevaliInstruct.Api.Services;
using RevaliInstruct.Core.Data;

// CORS policy name constant
const string MyAllowedOrigins = "MyAllowedOrigins";

var builder = WebApplication.CreateBuilder(args);

// =====================
// .env handmatig inladen
// =====================
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

// =====================
// Services registreren
// =====================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger + JWT definitie
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
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
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

// Configuratiebronnen laden
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json",
                 optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// =====================
// JWT Authenticatie
// =====================
var secret = builder.Configuration["Jwt:Key"] 
             ?? builder.Configuration["Jwt:Secret"] 
             ?? builder.Configuration["JWT__Secret"] 
             ?? Environment.GetEnvironmentVariable("JWT_SECRET");

if (string.IsNullOrEmpty(secret)) {
    Console.WriteLine("FOUT: Geen JWT Secret gevonden!");
}

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
            NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier,
            RoleClaimType = System.Security.Claims.ClaimTypes.Role

        };

        if (string.IsNullOrEmpty(secret))
        {
            Console.WriteLine("WAARSCHUWING: JWT Secret is leeg! Controleer je .env of appsettings.json.");
        }

        // Logging / debugging hooks
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                try
                {
                    var hdr = ctx.Request.Headers["Authorization"].FirstOrDefault()
                              ?? ctx.Request.Headers["authorization"].FirstOrDefault();

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

// =====================
// Database connectie
// =====================
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
        options.UseSqlServer(conn)
               .EnableDetailedErrors()
               .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
    );
}
else
{
    Console.WriteLine("Geen DefaultConnection gevonden");
}

// =====================
// App builden
// =====================
var app = builder.Build();

// Unieke server instance id voor debugging
var serverInstanceId = Guid.NewGuid().ToString();
Console.WriteLine($"Server instance id: {serverInstanceId}");

// Header middleware
app.Use(async (ctx, next) =>
{
    ctx.Response.OnStarting(() =>
    {
        ctx.Response.Headers["X-Server-Instance"] = serverInstanceId;
        ctx.Response.Headers.Append("Access-Control-Expose-Headers", "X-Server-Instance");
        return Task.CompletedTask;
    });

    await next();
});

// =====================
// Optionele reset via env
// =====================
if (Environment.GetEnvironmentVariable("RESET_DB") == "1")
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DbInitializer.ResetAndSeedAsync(db);
    Console.WriteLine("Database reset (RESET_DB=1).");
}

// =====================
// Automatische seeding in Development
// =====================
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

// =====================
// Error handling
// =====================
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

// =====================
// Swagger in dev
// =====================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// =====================
// Pipeline volgorde
// =====================
app.UseRouting();

app.UseCors(MyAllowedOrigins);

app.UseAuthentication();

// middleware die SQL SESSION_CONTEXT zet
app.UseMiddleware<SqlSessionContextMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();