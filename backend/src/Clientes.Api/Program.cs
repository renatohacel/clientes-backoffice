using Clientes.Infrasctructure.Persistencia;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ClientesDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("ClientesDb")));


var app = builder.Build();

app.MapGet("/", () => "Clientes API");

app.Run();
