using System;
using System.Data;
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


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();


// Add services to the container.
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
builder.Services.AddSwaggerGen();





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

app.UseAuthorization();

app.MapControllers();

app.Run();
