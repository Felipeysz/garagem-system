using GRA.Domain.Enums;

namespace GRA.Domain.Entities;

public class Orcamento
{
    public int Id { get; set; }
    public int GaragemId { get; set; }
    public Garagem? Garagem { get; set; }
    public int OrdemServicoId { get; set; }
    public OrdemServico? OrdemServico { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAprovacao { get; set; }
    public StatusOrcamento Status { get; set; }
    public string? Observacoes { get; set; }
    public ICollection<OrcamentoItem> Itens { get; set; } = new List<OrcamentoItem>();
    public decimal ValorTotal => Itens.Sum(i => i.ValorTotal);
}