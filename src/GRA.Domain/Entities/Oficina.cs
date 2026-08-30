using GRA.Domain.Entities.Commom;
using GRA.Domain.ValueObjects;

namespace GRA.Domain.Entities;

public class Oficina : Entity
{
    public required string Nome { get; set; }
    public string Slug { get; set; } = string.Empty;
    public required Cnpj CNPJ { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public Endereco? Endereco { get; set; }
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    public bool Ativo { get; set; } = true;

    public ICollection<Cliente> Clientes { get; set; } = [];
    public ICollection<Veiculo> Veiculos { get; set; } = [];
    public ICollection<Funcionario> Funcionarios { get; set; } = [];
    public ICollection<TipoServico> TiposServico { get; set; } = [];
    public ICollection<Servico> Servicos { get; set; } = [];
    public ICollection<Fornecedor> Fornecedores { get; set; } = [];
    public ICollection<Peca> Pecas { get; set; } = [];
    public ICollection<OrdemServico> OrdensServico { get; set; } = [];
}