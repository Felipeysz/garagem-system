using GRA.Domain.Entities.Commom;

namespace GRA.Domain.Entities;

public class Peca : Entity
{
    public long OficinaId { get; set; }
    public Oficina Oficina { get; set; } = null!;

    public required string Nome { get; set; }
    public string? Descricao { get; set; }
    public string? CodigoInterno { get; set; }
    public string? UnidadeMedida { get; set; }
    public decimal? PrecoVenda { get; set; }
    public int EstoqueMinimo { get; set; }
    public bool Ativo { get; set; } = true;

    public ICollection<MovimentacaoEstoque> Movimentacoes { get; set; } = [];
}