using GRA.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GRA.Infra.Persistence.Context;

public class GRAContext : DbContext
{
    public GRAContext(DbContextOptions<GRAContext> options) : base(options)
    {
    }

    public DbSet<Garagem> Garagens => Set<Garagem>();
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
        modelBuilder.Entity<Garagem>(e =>
        {
            e.HasIndex(g => g.CNPJ).IsUnique();
        });

        modelBuilder.Entity<Cliente>(e =>
        {
            e.HasIndex(c => new { c.GaragemId, c.CPF }).IsUnique();

            e.HasOne(c => c.Garagem)
                .WithMany(g => g.Clientes)
                .HasForeignKey(c => c.GaragemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Veiculo>(e =>
        {
            e.HasIndex(v => new { v.GaragemId, v.Placa }).IsUnique();
            e.HasIndex(v => new { v.GaragemId, v.Chassi }).IsUnique();

            e.HasOne(v => v.Garagem)
                .WithMany(g => g.Veiculos)
                .HasForeignKey(v => v.GaragemId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(v => v.Cliente)
                .WithMany(c => c.Veiculos)
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Funcionario>(e =>
        {
            e.HasIndex(f => new { f.GaragemId, f.CPF }).IsUnique();

            e.HasOne(f => f.Garagem)
                .WithMany(g => g.Funcionarios)
                .HasForeignKey(f => f.GaragemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TipoServico>(e =>
        {
            e.HasIndex(t => new { t.GaragemId, t.Nome }).IsUnique();

            e.HasOne(t => t.Garagem)
                .WithMany(g => g.TiposServico)
                .HasForeignKey(t => t.GaragemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Servico>(e =>
        {
            e.HasIndex(s => new { s.GaragemId, s.Nome }).IsUnique();

            e.HasOne(s => s.Garagem)
                .WithMany(g => g.Servicos)
                .HasForeignKey(s => s.GaragemId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(s => s.TipoServico)
                .WithMany(t => t.Servicos)
                .HasForeignKey(s => s.TipoServicoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Fornecedor>(e =>
        {
            e.HasIndex(f => new { f.GaragemId, f.CNPJ }).IsUnique();

            e.HasOne(f => f.Garagem)
                .WithMany(g => g.Fornecedores)
                .HasForeignKey(f => f.GaragemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Peca>(e =>
        {
            e.HasIndex(p => new { p.GaragemId, p.Nome }).IsUnique();
            e.Property(p => p.PrecoVenda).HasPrecision(10, 2);

            e.HasOne(p => p.Garagem)
                .WithMany(g => g.Pecas)
                .HasForeignKey(p => p.GaragemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OrdemServico>(e =>
        {
            e.HasOne(o => o.Garagem)
                .WithMany(g => g.OrdensServico)
                .HasForeignKey(o => o.GaragemId)
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

            e.HasOne(o => o.Garagem)
                .WithMany()
                .HasForeignKey(o => o.GaragemId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(o => o.OrdemServico)
                .WithOne(os => os.Orcamento)
                .HasForeignKey<Orcamento>(o => o.OrdemServicoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OrdemServicoServico>(e =>
        {
            e.HasIndex(os => new { os.OrdemServicoId, os.ServicoId }).IsUnique();

            e.HasOne(os => os.Garagem)
                .WithMany()
                .HasForeignKey(os => os.GaragemId)
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

            e.HasOne(m => m.Garagem)
                .WithMany()
                .HasForeignKey(m => m.GaragemId)
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
