using Clientes.Domain.Clientes;
using Clientes.Domain.Comunes;
using FluentAssertions;

namespace Clientes.Tests.Dominio;

public class ClienteTests
{
    private static Cliente CrearClienteDePrueba() =>
        Cliente.Crear("María López", "LOFM850312AB1", "maria@test.com", "3312345678");

    [Fact]
    public void Crear_AsignaLasInvariantesDelSistema()
    {
        var antes = DateTime.UtcNow;
        var cliente = CrearClienteDePrueba();

        cliente.Id.Should().NotBeEmpty();
        cliente.Estado.Should().Be(EstadoCliente.Activo);
        cliente.FechaAlta.Should().BeOnOrAfter(antes)
            .And.BeOnOrBefore(DateTime.UtcNow);
    }

    [Fact]
    public void Crear_NormalizaRfcYEmail()
    {
        var cliente = Cliente.Crear("  Juan Pérez  ", "pepj800101abc", "JUAN@TEST.COM", null);

        cliente.Nombre.Should().Be("Juan Pérez");
        cliente.Rfc.Should().Be("PEPJ800101ABC");
        cliente.Email.Should().Be("juan@test.com");
        cliente.Telefono.Should().BeNull();
    }

    [Fact]
    public void CambiarEstado_ConTransicionValida_CambiaElEstado()
    {
        var cliente = CrearClienteDePrueba();

        cliente.CambiarEstado(EstadoCliente.Suspendido);

        cliente.Estado.Should().Be(EstadoCliente.Suspendido);
    }

    [Fact]
    public void CambiarEstado_ConTransicionInvalida_LanzaReglaDeNegocio()
    {
        var cliente = CrearClienteDePrueba();
        cliente.CambiarEstado(EstadoCliente.Inactivo); // Activo → Inactivo, válida

        var acto = () => cliente.CambiarEstado(EstadoCliente.Suspendido); // prohibida

        acto.Should().Throw<ReglaDeNegocioException>()
            .Which.Codigo.Should().Be("transicion_invalida");
    }

    [Fact]
    public void CambiarEstado_ConTransicionInvalida_NoModificaElEstado()
    {
        var cliente = CrearClienteDePrueba();
        cliente.CambiarEstado(EstadoCliente.Inactivo);

        try { cliente.CambiarEstado(EstadoCliente.Suspendido); }
        catch (ReglaDeNegocioException) { }

        cliente.Estado.Should().Be(EstadoCliente.Inactivo); // intacto
    }

    [Fact]
    public void ActualizarDatos_NormalizaIgualQueLaCreacion()
    {
        var cliente = CrearClienteDePrueba();

        cliente.ActualizarDatos("Ana Ruiz", "ruia900101xy2", "ANA@TEST.COM", "  ");

        cliente.Rfc.Should().Be("RUIA900101XY2");
        cliente.Email.Should().Be("ana@test.com");
        cliente.Telefono.Should().BeNull(); // espacios = sin teléfono
    }
}