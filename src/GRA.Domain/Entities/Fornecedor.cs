using GRA.Domain.Entities.Commom;

namespace GRA.Domain.Entities;

public class Fornecedor : Entity
{
    public long OficinaId { get; set; }
    public Oficina? Oficina { get; set; }

    public required string RazaoSocial { get; set; }
    public string? NomeFantasia { get; set; }
    public required string CNPJ { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string? Endereco { get; set; }
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    public bool Ativo { get; set; } = true;

    public ICollection<MovimentacaoEstoque> Movimentacoes { get; set; } = [];
}
