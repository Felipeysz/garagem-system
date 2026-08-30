using GRA.Domain.Entities;
using GRA.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GRA.Infra.Persistence.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.CPF)
            .HasConversion(
                cpf => cpf.Valor,
                valor => Cpf.Parse(valor))
            .IsRequired()
            .HasMaxLength(11);
    }
}