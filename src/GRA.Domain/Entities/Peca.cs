namespace GRA.Domain.Entities;

public class Peca
{
    public int Id { get; set; }
    public int GaragemId { get; set; }
    public Garagem? Garagem { get; set; }
    public string Nome { get; set; } = null!;
    public string? Descricao { get; set; }
    public string? CodigoInterno { get; set; }
    public string? UnidadeMedida { get; set; }
    public decimal? PrecoVenda { get; set; }
    public int EstoqueMinimo { get; set; }
    public int SaldoAtual { get; set; }
    public bool Ativo { get; set; }
    public ICollection<MovimentacaoEstoque> Movimentacoes { get; set; } = new List<MovimentacaoEstoque>();
}