using System;
using System.Data;
using System.Text;
using AutoMapper;
using Orders.Dal.Repository;
using Orders.Dal.Repository.Interfaces;
using Orders.Dal.Context.Interfaces;
using Orders.Dal.Context;
using Orders.Dal.UoW;
using Orders.Dal.UoW.Interfaces;
using Microsoft.Data.SqlClient;
using Orders.Bll.Mapper.Profiles;
using Microsoft.EntityFrameworkCore;
using Orders.Bll.Services;
using Orders.Bll.Services.Interfaces;
using Orders.Shared.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "YourApiIssuer",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "YourApiAudience",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] ?? "your-super-secret-key-with-at-least-32-characters!"))
        };
        
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault();
                Console.WriteLine($"Token received: {token?.Substring(0, Math.Min(50, token?.Length ?? 0))}...");
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine($"Challenge: {context.Error}, {context.ErrorDescription}");
                return Task.CompletedTask;
            }
        };
    });

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IDapperContext, DapperContext>();
builder.Services.AddScoped<IRestaurantContext, FakeRestaurantContext>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
builder.Services.AddScoped<IOrderDishOptionRepository, OrderDishOptionRepository>();
builder.Services.AddScoped<IOrderDishRepository, OrderDishRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IOrderStatusHistoryRepository, OrderStatusHistoryRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IAdminAnalyticsService, AdminAnalyticsService>();
builder.Services.AddScoped<IRestaurantAnalyticsService, RestaurantAnalyticsService>();
builder.Services.AddAutoMapper(typeof(OrderProfile));

builder.Services.AddControllers();
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OrdersDb")));
builder.Services.AddScoped((s) => new SqlConnection(builder.Configuration.GetConnectionString("OrdersDb")));

builder.Services.AddScoped<IDbTransaction>(s =>
{
    SqlConnection conn = s.GetRequiredService<SqlConnection>();
    conn.Open();
    return conn.BeginTransaction();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Додайте підтримку JWT в Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your token"
    });
    
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    db.Database.Migrate();
}

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ========== ВАЖЛИВО: ПОРЯДОК МАЄ БУТИ ТАКИЙ ==========
app.UseAuthentication(); // ← СПОЧАТКУ аутентифікація
app.UseAuthorization();  // ← ПОТІМ авторизація

app.MapControllers();

app.Run();