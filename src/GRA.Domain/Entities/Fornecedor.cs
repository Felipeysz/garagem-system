namespace GRA.Domain.Entities;

public class Fornecedor
{
    public int Id { get; set; }
    public int GaragemId { get; set; }
    public Garagem? Garagem { get; set; }
    public string RazaoSocial { get; set; } = null!;
    public string? NomeFantasia { get; set; }
    public string CNPJ { get; set; } = null!;
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string? Endereco { get; set; }
    public DateTime DataCadastro { get; set; }
    public bool Ativo { get; set; }
    public ICollection<MovimentacaoEstoque> Movimentacoes { get; set; } = new List<MovimentacaoEstoque>();
}