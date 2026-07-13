using Clientes.Domain.Clientes;
using FluentAssertions;

namespace Clientes.Tests.Dominio;

public class TransicionesEstadoClienteTests
{
    [Theory]
    [InlineData(EstadoCliente.Activo, EstadoCliente.Inactivo, true)]
    [InlineData(EstadoCliente.Activo, EstadoCliente.Suspendido, true)]
    [InlineData(EstadoCliente.Inactivo, EstadoCliente.Activo, true)]
    [InlineData(EstadoCliente.Suspendido, EstadoCliente.Activo, true)]
    [InlineData(EstadoCliente.Inactivo, EstadoCliente.Suspendido, false)]
    [InlineData(EstadoCliente.Suspendido, EstadoCliente.Inactivo, false)]
    public void EsValida_RespetaLaTablaDeTransiciones(
        EstadoCliente desde, EstadoCliente hacia, bool esperado)
    {
        TransicionesEstadoCliente.EsValida(desde, hacia).Should().Be(esperado);
    }

    [Theory]
    [InlineData(EstadoCliente.Activo)]
    [InlineData(EstadoCliente.Inactivo)]
    [InlineData(EstadoCliente.Suspendido)]
    public void EsValida_RechazaLaTransicionAlMismoEstado(EstadoCliente estado)
    {
        TransicionesEstadoCliente.EsValida(estado, estado).Should().BeFalse();
    }

    [Fact]
    public void DestinosDesde_DevuelveExactamenteLasTransicionesDeLaTabla()
    {
        TransicionesEstadoCliente.DestinosDesde(EstadoCliente.Activo)
            .Should().BeEquivalentTo([EstadoCliente.Inactivo, EstadoCliente.Suspendido]);

        TransicionesEstadoCliente.DestinosDesde(EstadoCliente.Inactivo)
            .Should().BeEquivalentTo([EstadoCliente.Activo]);

        TransicionesEstadoCliente.DestinosDesde(EstadoCliente.Suspendido)
            .Should().BeEquivalentTo([EstadoCliente.Activo]);
    }
}