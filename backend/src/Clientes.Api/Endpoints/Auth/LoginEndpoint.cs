using FastEndpoints;
using FastEndpoints.Security;

namespace Clientes.Api.Endpoints.Auth;

public class LoginRequest
{
    public string Usuario { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
}

public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var usuarioValido = Config["Auth:Usuario"];
        var passwordValido = Config["Auth:Password"];

        if (req.Usuario != usuarioValido || req.Password != passwordValido)
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var token = JwtBearer.CreateToken(opciones =>
        {
            opciones.SigningKey = Config["Jwt:SigningKey"]!;
            opciones.ExpireAt = DateTime.UtcNow.AddHours(8);
            opciones.User.Roles.Add("Operador");
            opciones.User.Claims.Add(new("usuario", req.Usuario));
        });

        await Send.ResponseAsync(new LoginResponse { Token = token }, cancellation: ct);
    }
}