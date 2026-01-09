using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RevaliInstruct.Api.Middleware;
using RevaliInstruct.Core.Data;
using RevaliInstruct.Api.Services;


const string MyAllowedOrigins = "MyAllowedOrigins";

var builder = WebApplication.CreateBuilder(args);

string currentDir = Directory.GetCurrentDirectory();
string[] pathsToTry = {
    Path.Combine(currentDir, ".env"),                          
    Path.Combine(currentDir, "..", ".env"),               
    Path.Combine(currentDir, "..", "..", ".env"),             
    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".env")
};

bool envLoaded = false;
foreach (var path in pathsToTry)
{
    var fullPath = Path.GetFullPath(path);
    if (File.Exists(fullPath))
    {
        Console.WriteLine($"[Config] .env gevonden op: {fullPath}");
        foreach (var line in File.ReadAllLines(fullPath))
        {
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#")) continue;
            var parts = line.Split('=', 2);
            if (parts.Length == 2)
            {
                var key = parts[0].Trim();
                var value = parts[1].Trim();
                
                Environment.SetEnvironmentVariable(key, value);
                builder.Configuration[key] = value;
            }
        }
        envLoaded = true;
        break; 
    }
}

if (!envLoaded) Console.WriteLine("[WAARSCHUWING] Geen .env bestand gevonden!");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Swagger configuratie
builder.Services.AddSwaggerGen(c =>
{
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
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// CORS voor de frontend (Vite)
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

// =====================
//  JWT Authenticatie
// =====================
var secret = builder.Configuration["JWT_SECRET"] 
             ?? builder.Configuration["Jwt:Key"] 
             ?? Environment.GetEnvironmentVariable("JWT_SECRET");

if (string.IsNullOrEmpty(secret)) {
    Console.WriteLine("FOUT: Geen JWT Secret gevonden!");
} else {
    Console.WriteLine($"SUCCES: JWT Secret geladen (lengte: {secret.Length})");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret ?? "fallback_secret_voor_build_doeleinden")),
        ClockSkew = TimeSpan.FromSeconds(30),
        NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier,
        RoleClaimType = System.Security.Claims.ClaimTypes.Role
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = ctx => {
            var hdr = ctx.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(hdr) && hdr.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)) {
                ctx.Token = hdr.Substring("Bearer ".Length).Trim();
            }
            return Task.CompletedTask;
        },
        OnTokenValidated = ctx => {
            var id = ctx.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine($"JwtBearer token validated. NameId={id}");
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = ctx => {
            Console.WriteLine("JwtBearer authentication failed: " + ctx.Exception?.Message);
            return Task.CompletedTask;
        }
    };
});

// =====================
//  Database connectie
// =====================
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(conn))
{
    var host = Environment.GetEnvironmentVariable("MSSQL_HOST") ?? "localhost";
    var db = Environment.GetEnvironmentVariable("MSSQL_DATABASE") ?? "revali_db";
    var user = Environment.GetEnvironmentVariable("MSSQL_USER") ?? "revali_login";
    var pwd = Environment.GetEnvironmentVariable("MSSQL_PASSWORD") ?? "";
    conn = $"Server={host},1433;Database={db};User Id={user};Password={pwd};TrustServerCertificate=True;MultipleActiveResultSets=true;";
}

Console.WriteLine($"DEBUG: DefaultConnection string samengesteld.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(conn).EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
);

// =====================
//  App Build & Middleware
// =====================
var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseCors(MyAllowedOrigins);
app.UseAuthentication();
app.UseMiddleware<SqlSessionContextMiddleware>();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (Environment.GetEnvironmentVariable("RESET_DB") == "1") {
        await DbInitializer.ResetAndSeedAsync(db);
        Console.WriteLine("Database reset uitgevoerd.");
    } else {
        await DbInitializer.SeedAsync(db);
    }
}

app.MapControllers();
app.Run();