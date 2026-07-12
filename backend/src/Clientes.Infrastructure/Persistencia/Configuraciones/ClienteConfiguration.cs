using Clientes.Domain.Clientes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clientes.Infrasctructure.Persistencia.Configuraciones;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("clientes");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nombre)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(c => c.Rfc)
            .HasMaxLength(13)
            .IsRequired();

        builder.Property(c => c.Email)
            .HasMaxLength(254)
            .IsRequired();

        builder.Property(c => c.Telefono)
            .HasMaxLength(20);

        builder.Property(c => c.Estado)
            .IsRequired();

        builder.Property(c => c.FechaAlta)
            .IsRequired();

        builder.HasIndex(c => c.Rfc).IsUnique();
        builder.HasIndex(c => c.Email).IsUnique();

        builder.Property(c => c.Version).IsRowVersion();
    }
}