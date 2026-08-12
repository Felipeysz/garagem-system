namespace GRA.Domain.Entities;

public class Peca
{
    public int Id { get; set; }

    public int GaragemId { get; set; }
    public Garagem? Garagem { get; set; }

    public required string Nome { get; set; }
    public string? Descricao { get; set; }
    public string? CodigoInterno { get; set; }
    public string? UnidadeMedida { get; set; }
    public decimal? PrecoVenda { get; set; }
    public int EstoqueMinimo { get; set; }
    public bool Ativo { get; set; } = true;

    public ICollection<MovimentacaoEstoque> Movimentacoes { get; set; } = [];
}
