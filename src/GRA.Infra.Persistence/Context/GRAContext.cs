using GRA.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GRA.Infra.Persistence.Context;

public class GRAContext : DbContext
{
    public GRAContext(DbContextOptions<GRAContext> options) : base(options)
    {
    }

    public DbSet<Oficina> Garagens => Set<Oficina>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Veiculo> Veiculos => Set<Veiculo>();
    public DbSet<Funcionario> Funcionarios => Set<Funcionario>();
    public DbSet<TipoServico> TiposServico => Set<TipoServico>();
    public DbSet<Servico> Servicos => Set<Servico>();
    public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();
    public DbSet<Peca> Pecas => Set<Peca>();
    public DbSet<OrdemServico> OrdensServico => Set<OrdemServico>();
    public DbSet<Orcamento> Orcamentos => Set<Orcamento>();
    public DbSet<OrdemServicoServico> OrdensServicoServico => Set<OrdemServicoServico>();
    public DbSet<MovimentacaoEstoque> MovimentacoesEstoque => Set<MovimentacaoEstoque>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Oficina>(e =>
        {
            e.HasIndex(g => g.CNPJ).IsUnique();
        });

        modelBuilder.Entity<Cliente>(e =>
        {
            e.HasIndex(c => new { c.OficinaId, c.CPF }).IsUnique();

            e.HasOne(c => c.Oficina)
                .WithMany(g => g.Clientes)
                .HasForeignKey(c => c.OficinaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Veiculo>(e =>
        {
            e.HasIndex(v => new { v.OficinaId, v.Placa }).IsUnique();
            e.HasIndex(v => new { v.OficinaId, v.Chassi }).IsUnique();

            e.HasOne(v => v.Oficina)
                .WithMany(g => g.Veiculos)
                .HasForeignKey(v => v.OficinaId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(v => v.Cliente)
                .WithMany(c => c.Veiculos)
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Funcionario>(e =>
        {
            e.HasIndex(f => new { f.OficinaId, f.CPF }).IsUnique();

            e.HasOne(f => f.Oficina)
                .WithMany(g => g.Funcionarios)
                .HasForeignKey(f => f.OficinaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TipoServico>(e =>
        {
            e.HasIndex(t => new { t.OficinaId, t.Nome }).IsUnique();

            e.HasOne(t => t.Oficina)
                .WithMany(g => g.TiposServico)
                .HasForeignKey(t => t.OficinaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Servico>(e =>
        {
            e.HasIndex(s => new { s.OficinaId, s.Nome }).IsUnique();

            e.HasOne(s => s.Oficina)
                .WithMany(g => g.Servicos)
                .HasForeignKey(s => s.OficinaId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(s => s.TipoServico)
                .WithMany(t => t.Servicos)
                .HasForeignKey(s => s.TipoServicoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Fornecedor>(e =>
        {
            e.HasIndex(f => new { f.OficinaId, f.CNPJ }).IsUnique();

            e.HasOne(f => f.Oficina)
                .WithMany(g => g.Fornecedores)
                .HasForeignKey(f => f.OficinaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Peca>(e =>
        {
            e.HasIndex(p => new { p.OficinaId, p.Nome }).IsUnique();
            e.Property(p => p.PrecoVenda).HasPrecision(10, 2);

            e.HasOne(p => p.Oficina)
                .WithMany(g => g.Pecas)
                .HasForeignKey(p => p.OficinaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OrdemServico>(e =>
        {
            e.HasOne(o => o.Oficina)
                .WithMany(g => g.OrdensServico)
                .HasForeignKey(o => o.OficinaId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(o => o.Veiculo)
                .WithMany(v => v.OrdensServico)
                .HasForeignKey(o => o.VeiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(o => o.FuncionarioResponsavel)
                .WithMany(f => f.OrdensServicoResponsavel)
                .HasForeignKey(o => o.FuncionarioResponsavelId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Orcamento>(e =>
        {
            e.HasIndex(o => o.OrdemServicoId).IsUnique();

            e.HasOne(o => o.Oficina)
                .WithMany()
                .HasForeignKey(o => o.OficinaId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(o => o.OrdemServico)
                .WithOne(os => os.Orcamento)
                .HasForeignKey<Orcamento>(o => o.OrdemServicoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OrdemServicoServico>(e =>
        {
            e.HasIndex(os => new { os.OrdemServicoId, os.ServicoId }).IsUnique();

            e.HasOne(os => os.Oficina)
                .WithMany()
                .HasForeignKey(os => os.OficinaId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(os => os.OrdemServico)
                .WithMany(o => o.Servicos)
                .HasForeignKey(os => os.OrdemServicoId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(os => os.Servico)
                .WithMany(s => s.OrdensServico)
                .HasForeignKey(os => os.ServicoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<MovimentacaoEstoque>(e =>
        {
            e.Property(m => m.PrecoUnitario).HasPrecision(10, 2);
            e.Property(m => m.Tipo).HasConversion<string>();

            e.HasOne(m => m.Oficina)
                .WithMany()
                .HasForeignKey(m => m.OficinaId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(m => m.Peca)
                .WithMany(p => p.Movimentacoes)
                .HasForeignKey(m => m.PecaId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(m => m.Fornecedor)
                .WithMany(f => f.Movimentacoes)
                .HasForeignKey(m => m.FornecedorId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(m => m.OrdemServico)
                .WithMany(o => o.Movimentacoes)
                .HasForeignKey(m => m.OrdemServicoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        base.OnModelCreating(modelBuilder);
    }
}
