using GRA.Domain.Entities.Commom;

namespace GRA.Domain.Entities;

public class Oficina : Entity
{
    public string Nome { get; set; }
    public string CNPJ { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string? Endereco { get; set; }
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
