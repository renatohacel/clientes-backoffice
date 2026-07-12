using Clientes.Api.Middleware;
using Clientes.Application.Abstracciones;
using Clientes.Application.Clientes.CasosDeUso;
using Clientes.Infrasctructure.Persistencia;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//Persistencia
builder.Services.AddDbContext<ClientesDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("ClientesDb")));

builder.Services.AddScoped<IClientesRepository, ClientesRepository>();

//Casos de uso
builder.Services.AddScoped<CrearCliente>();
builder.Services.AddScoped<ActualizarCliente>();
builder.Services.AddScoped<CambiarEstadoCliente>();
builder.Services.AddScoped<ObtenerCliente>();
builder.Services.AddScoped<ListarClientes>();

//Seguridad + FastEndpoints + Sawgger
builder.Services
    .AddAuthenticationJwtBearer(firma => firma.SigningKey = builder.Configuration["Jwt:SigningKey"])
    .AddAuthorization()
    .AddFastEndpoints()
    .SwaggerDocument();

//CORS
var corsOrigins = (builder.Configuration["Cors:AllowedOrigins"]
    ?? throw new InvalidOperationException("Falta la configuración 'Cors:AllowedOrigins'."))
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(opciones =>
    opciones.AddPolicy("Frontend", politica =>
        politica.WithOrigins(corsOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()));

var app = builder.Build();

app.UseMiddleware<ManejoErroresMiddleware>();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints(config => config.Endpoints.RoutePrefix = "api");
app.UseSwaggerGen();

app.Run();
