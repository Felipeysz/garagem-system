namespace GRA.Domain.Entities;

public class Garagem
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string CNPJ { get; set; } = null!;
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string? Endereco { get; set; }
    public DateTime DataCadastro { get; set; }
    public bool Ativo { get; set; }

    public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
    public ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
    public ICollection<Funcionario> Funcionarios { get; set; } = new List<Funcionario>();
    public ICollection<TipoServico> TiposServico { get; set; } = new List<TipoServico>();
    public ICollection<Servico> Servicos { get; set; } = new List<Servico>();
    public ICollection<Fornecedor> Fornecedores { get; set; } = new List<Fornecedor>();
    public ICollection<Peca> Pecas { get; set; } = new List<Peca>();
    public ICollection<OrdemServico> OrdensServico { get; set; } = new List<OrdemServico>();
}