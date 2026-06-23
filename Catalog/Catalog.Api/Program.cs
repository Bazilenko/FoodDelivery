using System.Text;
using Catalog.Api.Middleware;
using Catalog.Bll.Mapper.Profiles;
using Catalog.Bll.Services;
using Catalog.Bll.Services.Interfaces;
using Catalog.Dal.Context;
using Catalog.Dal.UOW;
using Catalog.Dal.UOW.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// ── Database ──────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CatalogDb"))
);

// ── JWT Authentication ────────────────────────────────────────────────────────
var jwtSection = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSection["SecretKey"] ?? throw new InvalidOperationException("SecretKey is missing in appsettings.json");
var issuer = jwtSection["Issuer"];
var audience = jwtSection["Audience"];

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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor(); // Необхідно для роботи AuthHelper

// ── Http Clients ──────────────────────────────────────────────────────────────
builder.Services.AddHttpClient<IIdentityClient, IdentityClient>(client =>
{
    // Беремо URL мікросервісу Identity з конфігурації Aspire або appsettings
    var identityUrl = builder.Configuration["Services:IdentityUri"] ?? "https://localhost:7195/";
    client.BaseAddress = new Uri(identityUrl);
});

// ── Application Services ──────────────────────────────────────────────────────
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IDishOwnerService, DishOwnerService>();
builder.Services.AddScoped<IDishPublicService, DishPublicService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<ICuisineService, CuisineService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IDishOptionService, DishOptionService>();
builder.Services.AddScoped<IModifierGroupService, ModifierGroupService>();
builder.Services.AddScoped<IRestaurantOwnerService, RestaurantOwnerService>();
builder.Services.AddScoped<IRestaurantPublicService, RestaurantPublicService>();
builder.Services.AddScoped<IWorkingHourService, WorkingHourService>();
builder.Services.AddScoped<IRestaurantMenuService, RestaurantMenuService>();

// ── AutoMapper ────────────────────────────────────────────────────────────────
builder.Services.AddAutoMapper(typeof(CategoryProfile));
builder.Services.AddAutoMapper(typeof(DishProfile));
builder.Services.AddAutoMapper(typeof(RestaurantProfile));
builder.Services.AddAutoMapper(typeof(ContactProfile));
builder.Services.AddAutoMapper(typeof(AddressProfile));
builder.Services.AddAutoMapper(typeof(WorkingHourProfile));
builder.Services.AddAutoMapper(typeof(CuisineProfile));
builder.Services.AddAutoMapper(typeof(DishOptionProfile));
builder.Services.AddAutoMapper(typeof(ModifierGroupProfile));

// ── Controllers & Swagger ─────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog Service API", Version = "v1" });

    // Додаємо можливість авторизації через Bearer токен в інтерфейсі Swagger
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.Http,
        Scheme       = "Bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header,
        Description  = "Введіть JWT токен у форматі: Bearer {your_token}"
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

app.UseMiddleware<ExceptionMiddleware>();

// ── Database Migrations on Startup ────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<MyDbContext>();
    db.Database.Migrate();
}

app.MapDefaultEndpoints();

// ── Middleware Pipeline ───────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ПОРЯДОК КРИТИЧНИЙ: Спочатку автентифікація, потім авторизація!
app.UseAuthentication(); 
app.UseAuthorization();

app.UseCors(builder => builder
    .WithOrigins("http://localhost:5173", "https://localhost:5173")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());
app.MapControllers();

app.Run();