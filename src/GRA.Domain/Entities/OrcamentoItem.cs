namespace GRA.Domain.Entities;

public class OrcamentoItem
{
    public int Id { get; set; }
    public int GaragemId { get; set; }
    public Garagem? Garagem { get; set; }
    public int OrcamentoId { get; set; }
    public Orcamento? Orcamento { get; set; }
    public string Descricao { get; set; } = null!;
    public int Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal => Quantidade * ValorUnitario;
}