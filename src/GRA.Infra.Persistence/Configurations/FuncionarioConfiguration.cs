using GRA.Domain.Entities;
using GRA.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GRA.Infra.Persistence.Configurations;

public class FuncionarioConfiguration : IEntityTypeConfiguration<Funcionario>
{
    public void Configure(EntityTypeBuilder<Funcionario> builder)
    {
        builder.Property(f => f.Nome)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(f => f.CPF)
            .HasConversion(
                cpf => cpf.Valor,
                valor => Cpf.Parse(valor))
            .IsRequired()
            .HasMaxLength(11);

        builder.Property(f => f.Cargo)
            .IsRequired()
            .HasMaxLength(100);
    }
}