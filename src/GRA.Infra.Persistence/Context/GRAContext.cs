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
}