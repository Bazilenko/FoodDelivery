using System.Text;
using Auth.Entities;
using Auth.Services;
using Auth.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Auth.Context;
using AuthService.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;


var builder = WebApplication.CreateBuilder(args);

// ── Database (Aspire injects "IdentityDb" connection string) ──────────────────

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityDb")));

// ── ASP.NET Identity ──────────────────────────────────────────────────────────

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole<int>>(opt =>
    {
        // Password requirements
        opt.Password.RequiredLength         = 8;
        opt.Password.RequireDigit           = true;
        opt.Password.RequireUppercase       = true;
        opt.Password.RequireNonAlphanumeric = true;

        // Lockout — 5 failed attempts → locked for 15 minutes
        opt.Lockout.MaxFailedAccessAttempts = 5;
        opt.Lockout.DefaultLockoutTimeSpan  = TimeSpan.FromMinutes(15);
        opt.Lockout.AllowedForNewUsers      = true;

        // Require unique email
        opt.User.RequireUniqueEmail = true;

        // Require email confirmation before login
        opt.SignIn.RequireConfirmedEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// ── JWT Settings ──────────────────────────────────────────────────────────────

var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtSettings>(jwtSection);
var jwtSettings = jwtSection.Get<JwtSettings>()!;

// ── JWT Authentication ────────────────────────────────────────────────────────

builder.Services
    .AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = new SymmetricSecurityKey(
                                           Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ValidateIssuer   = true,
            ValidIssuer      = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience    = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew        = TimeSpan.Zero   // no grace period — tokens expire exactly on time
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ── Application Services ──────────────────────────────────────────────────────

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IEmailService, DevEmailService>(); 

// ── Controllers & Swagger ─────────────────────────────────────────────────────

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth Service", Version = "v1" });

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.Http,
        Scheme       = "Bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                    { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// ── Build ─────────────────────────────────────────────────────────────────────

var app = builder.Build();

// ── Seed default roles and run migrations on startup ─────────────────────────

await SeedAsync(app);

// ── Middleware pipeline ───────────────────────────────────────────────────────

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("ReactPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// ── Seed helper ───────────────────────────────────────────────────────────────

static async Task SeedAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var db          = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

    // Apply any pending migrations automatically on startup
    await db.Database.MigrateAsync();

    // Ensure default roles exist
    string[] roles = ["User", "RestaurantOwner", "Admin", "Courier"];
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole<int>(role));
    }
}