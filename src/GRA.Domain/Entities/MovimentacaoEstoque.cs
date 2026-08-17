using GRA.Domain.Entities.Commom;
using GRA.Domain.Enums;

namespace GRA.Domain.Entities;

public class MovimentacaoEstoque : Entity
{
    public long OficinaId { get; set; }
    public Oficina? Oficina { get; set; }

    public long PecaId { get; set; }
    public Peca? Peca { get; set; }

    public long? FornecedorId { get; set; }
    public Fornecedor? Fornecedor { get; set; }

    public long? OrdemServicoId { get; set; }
    public OrdemServico? OrdemServico { get; set; }

    public TipoMovimentacaoEstoque Tipo { get; set; }
    public int Quantidade { get; set; }
    public decimal? PrecoUnitario { get; set; }

    public DateTime DataMovimentacao { get; set; } = DateTime.UtcNow;
    public string? Observacoes { get; set; }
}
