using GRA.Domain.Entities;
using GRA.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GRA.Infra.Persistence.Configurations;

public class OficinaConfiguration : IEntityTypeConfiguration<Oficina>
{
    public void Configure(EntityTypeBuilder<Oficina> builder)
    {
        builder.Property(o => o.Nome)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(o => o.Slug)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(o => o.CNPJ)
            .HasConversion(
                cnpj => cnpj.Valor,
                valor => Cnpj.Parse(valor))
            .IsRequired()
            .HasMaxLength(14);
    }
}