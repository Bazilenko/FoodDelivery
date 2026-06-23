using System.Text;
using Delivery.Api.Middleware;
using Delivery.Application.Behaviors;
using Delivery.Application.Commands.CourierCommands.Handlers;
using Delivery.Application.Commands.CourierCommands.Validator;
using Delivery.Application.Commands.DeliveryCommands.Command;
using Delivery.Application.Commands.DeliveryCommands.Handler;
using Delivery.Application.Services;
using Delivery.Application.Interfaces.Repositories;
using Delivery.Application.Interfaces.ExternalServices;
using Delivery.Infrastructure.ExternalServices;
using Delivery.Domain.Interfaces.Services;
using Delivery.Infrastructure.Mongo;
using Delivery.Infrastructure.Mongo.Config;
using Delivery.Infrastructure.Mongo.UoW;
using Delivery.Infrastructure.Repository;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var aspConn = builder.Configuration.GetConnectionString("DeliveryDb") ??
              builder.Configuration.GetConnectionString("mongodb");

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

if (!string.IsNullOrEmpty(aspConn))
{
    builder.Services.Configure<MongoDbSettings>(options =>
    {
        options.ConnectionString = aspConn;
        options.DatabaseName = "DeliveryDb";
        options.MaxConnectionPoolSize = 100;
        options.MinConnectionPoolSize = 5;
        options.ConnectTimeoutSeconds = 10;
        options.SocketTimeoutSeconds = 10;
    });
}

// Controllers
builder.Services.AddControllers();

// MongoDB
builder.Services.AddSingleton<MongoDbContext>();

builder.Services.AddScoped<IUnitOfWork>(provider =>
{
    var context = provider.GetRequiredService<MongoDbContext>();
    return new UnitOfWork(context.Database);
});

// Repositories
builder.Services.AddScoped<ICourierRepository, CourierRepository>();
builder.Services.AddScoped<IDeliveryRepository, DeliveryRepository>();

// Services
builder.Services.AddScoped<ICourierService, CourierService>();
builder.Services.AddScoped<IDeliveryService, DeliveryService>();

// External services
builder.Services.AddHttpClient<IOrderServiceClient, OrderServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:OrderService"] ?? "https://localhost:7295");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<IIdentityServiceClient, IdentityServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:IdentityService"] ?? "https://localhost:7296");
});

// MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(typeof(CreateCourierCommandHandler).Assembly);
    cfg.RegisterServicesFromAssemblies(typeof(AssignCourierToDeliveryCommandHandler).Assembly);
    cfg.RegisterServicesFromAssemblies(typeof(AssignDeliveryWindowCommand).Assembly);

    cfg.AddOpenBehavior(typeof(ExceptionHandlingBehavior<,>));
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(CreateCourierCommandValidator).Assembly);

// JWT Authentication — як в Orders
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
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:SecretKey"] ??
                    "your-super-secret-key-with-at-least-32-characters!"
                )
            )
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault();

                Console.WriteLine(
                    $"Token received: {token?.Substring(0, Math.Min(50, token?.Length ?? 0))}..."
                );

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

builder.Services.AddAuthorization();

// Swagger + JWT
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();