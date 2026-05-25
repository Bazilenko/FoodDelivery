using Microsoft.Extensions.Configuration;
using Projects;
using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);



var password = builder.AddParameter("msSql-password", "mssql123",secret: true);

//var userMsSql = builder.AddParameter("sql-username", "mssql", secret: true);

var sql = builder.AddSqlServer("mssql").WithDataVolume();

var mongo = builder.AddMongoDB("mongo")
    .WithDataVolume()
    .WithContainerRuntimeArgs("--restart", "on-failure"); // restart policy

var deliveryDb = mongo.AddDatabase("deliveryDb");

var ordersDb = sql.AddDatabase("ordersDb");

var catalogDb = sql.AddDatabase("catalogDb");

var identityDb = sql.AddDatabase("IdentityDb");

var authService = builder.AddProject<AuthService>("auth-service")
    .WithReference(identityDb)   
    .WaitFor(identityDb);

var deliveryService = builder.AddProject<Delivery_Api>("delivery-api")
    .WithReference(deliveryDb)
    .WaitFor(deliveryDb);


var catalogService = builder.AddProject<Catalog_Api>("catalog-api")
    .WithReference(catalogDb)
    .WaitFor(catalogDb);

var ordersService = builder.AddProject<Orders_Api>("orders-api")
    .WithReference(ordersDb)
    .WaitFor(ordersDb);



builder.AddProject<Gateway_Api>("gateway-api")
    .WithExternalHttpEndpoints()
    .WaitFor(catalogService)
    .WaitFor(deliveryService)
    .WaitFor(ordersService)
    .WaitFor(authService)
    .WithReference(deliveryService)
    .WithReference(catalogService)
    .WithReference(ordersService)
    .WithReference(authService);

builder.AddProject<Aggreagator_Api>("aggreagator-api")
    .WithExternalHttpEndpoints()
    .WithReference(deliveryService)
    .WithReference(catalogService)
    .WithReference(ordersService)
    .WaitFor(deliveryService)
    .WaitFor(catalogService)
    .WaitFor(ordersService);

builder.Build().Run();
