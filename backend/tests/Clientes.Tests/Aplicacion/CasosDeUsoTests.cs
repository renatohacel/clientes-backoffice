using Clientes.Application.Clientes.CasosDeUso;
using Clientes.Domain.Clientes;
using Clientes.Domain.Comunes;
using FluentAssertions;

namespace Clientes.Tests.Aplicacion;

public class CasosDeUsoTests
{
    private readonly RepositorioFalso _repo = new();

    [Fact]
    public async Task CrearCliente_ConDatosValidos_PersisteYDevuelveElDto()
    {
        var casoDeUso = new CrearCliente(_repo);

        var dto = await casoDeUso.EjecutarAsync(
            "María López", "lofm850312ab1", "MARIA@TEST.COM", null, CancellationToken.None);

        dto.Rfc.Should().Be("LOFM850312AB1");   // normalizado
        dto.Estado.Should().Be("Activo");
        dto.TransicionesPermitidas.Should().BeEquivalentTo(["Inactivo", "Suspendido"]);
        _repo.Clientes.Should().HaveCount(1);
        _repo.VecesGuardado.Should().Be(1);
    }

    [Fact]
    public async Task CrearCliente_ConRfcDuplicado_LanzaYNoPersiste()
    {
        _repo.Clientes.Add(Cliente.Crear("Otro", "LOFM850312AB1", "otro@test.com", null));
        var casoDeUso = new CrearCliente(_repo);

        var acto = () => casoDeUso.EjecutarAsync(
            "María López", "LOFM850312AB1", "maria@test.com", null, CancellationToken.None);

        (await acto.Should().ThrowAsync<ReglaDeNegocioException>())
            .Which.Codigo.Should().Be("rfc_duplicado");
        _repo.Clientes.Should().HaveCount(1);   // no se agregó el segundo
        _repo.VecesGuardado.Should().Be(0);     // nunca llegó a guardar
    }

    [Fact]
    public async Task CrearCliente_DetectaDuplicadoAunConDistintaCapitalizacion()
    {
        _repo.Clientes.Add(Cliente.Crear("Otro", "LOFM850312AB1", "maria@test.com", null));
        var casoDeUso = new CrearCliente(_repo);

        var acto = () => casoDeUso.EjecutarAsync(
            "María", "lofm850312ab1", "MARIA@TEST.COM", null, CancellationToken.None);

        await acto.Should().ThrowAsync<ReglaDeNegocioException>();
    }

    [Fact]
    public async Task ActualizarCliente_NoSeAcusaASiMismoDeDuplicado()
    {
        var cliente = Cliente.Crear("María", "LOFM850312AB1", "maria@test.com", null);
        _repo.Clientes.Add(cliente);
        var casoDeUso = new ActualizarCliente(_repo);

        // Guarda con su MISMO rfc y email: no debe lanzar duplicado.
        var dto = await casoDeUso.EjecutarAsync(
            cliente.Id, "María Fernanda", "LOFM850312AB1", "maria@test.com", "3312345678",
            version: 1, CancellationToken.None);

        dto.Nombre.Should().Be("María Fernanda");
        _repo.VecesGuardado.Should().Be(1);
    }

    [Fact]
    public async Task CambiarEstado_ConClienteInexistente_LanzaNoEncontrado()
    {
        var casoDeUso = new CambiarEstadoCliente(_repo);

        var acto = () => casoDeUso.EjecutarAsync(
            Guid.NewGuid(), EstadoCliente.Inactivo, version: 1, CancellationToken.None);

        await acto.Should().ThrowAsync<NoEncontradoException>();
    }

    [Fact]
    public async Task CambiarEstado_ConTransicionInvalida_LanzaYNoGuarda()
    {
        var cliente = Cliente.Crear("María", "LOFM850312AB1", "maria@test.com", null);
        cliente.CambiarEstado(EstadoCliente.Inactivo);
        _repo.Clientes.Add(cliente);
        var casoDeUso = new CambiarEstadoCliente(_repo);

        var acto = () => casoDeUso.EjecutarAsync(
            cliente.Id, EstadoCliente.Suspendido, version: 1, CancellationToken.None);

        (await acto.Should().ThrowAsync<ReglaDeNegocioException>())
            .Which.Codigo.Should().Be("transicion_invalida");
        _repo.VecesGuardado.Should().Be(0);
    }

    [Fact]
    public async Task CambiarEstado_ConTransicionValida_GuardaYDevuelveLasNuevasTransiciones()
    {
        var cliente = Cliente.Crear("María", "LOFM850312AB1", "maria@test.com", null);
        _repo.Clientes.Add(cliente);
        var casoDeUso = new CambiarEstadoCliente(_repo);

        var dto = await casoDeUso.EjecutarAsync(
            cliente.Id, EstadoCliente.Suspendido, version: 1, CancellationToken.None);

        dto.Estado.Should().Be("Suspendido");
        dto.TransicionesPermitidas.Should().BeEquivalentTo(["Activo"]);
        _repo.VecesGuardado.Should().Be(1);
    }
}