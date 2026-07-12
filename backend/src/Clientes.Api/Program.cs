using Clientes.Application.Abstracciones;
using Clientes.Application.Clientes.CasosDeUso;
using Clientes.Infrasctructure.Persistencia;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ClientesDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("ClientesDb")));

builder.Services.AddScoped<IClientesRepository, ClientesRepository>();

builder.Services.AddScoped<CrearCliente>();
builder.Services.AddScoped<ActualizarCliente>();
builder.Services.AddScoped<CambiarEstadoCliente>();
builder.Services.AddScoped<ObtenerCliente>();
builder.Services.AddScoped<ListarClientes>();

var app = builder.Build();

app.MapGet("/", () => "Clientes API");

app.Run();
