

using Catalog.Api.Middleware;
using Catalog.Bll.Mapper.Profiles;
using Catalog.Bll.Services;
using Catalog.Bll.Services.Interfaces;
using Catalog.Dal.Context;
using Catalog.Dal.Repositories;
using Catalog.Dal.Repositories.Interfaces;
using Catalog.Dal.UOW;
using Catalog.Dal.UOW.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CatalogDb"))
    );

builder.Services.AddScoped<IRestaurantContext,  FakeRestaurantContext>();

// Add services to the container.


builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IDishOwnerService,  DishOwnerService>();
builder.Services.AddScoped<IDishPublicService, DishPublicService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<ICuisineService, CuisineService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IDishOptionService,  DishOptionService>();
builder.Services.AddScoped<IModifierGroupService,  ModifierGroupService>();
builder.Services.AddScoped<IRestaurantOwnerService,  RestaurantOwnerService>();
builder.Services.AddScoped<IRestaurantPublicService,  RestaurantPublicService>();
builder.Services.AddScoped<IWorkingHourService,  WorkingHourService>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddAutoMapper(typeof(CategoryProfile));
builder.Services.AddAutoMapper(typeof(DishProfile));
builder.Services.AddAutoMapper(typeof(RestaurantProfile));
builder.Services.AddAutoMapper(typeof(ContactProfile));
builder.Services.AddAutoMapper(typeof(AddressProfile));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

//migration for data base in container
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<MyDbContext>();
    db.Database.Migrate();

    //For data seeding
    //var uow = services.GetRequiredService<IUnitOfWork>();
    //await SeedData.SeedAsync(uow);
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
