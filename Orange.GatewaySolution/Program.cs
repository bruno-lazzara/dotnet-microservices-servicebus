using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Orange.GatewaySolution.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddAppAuthentication();

builder.Services.AddOcelot();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.UseOcelot();

app.Run();
