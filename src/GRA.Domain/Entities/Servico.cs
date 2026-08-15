namespace GRA.Domain.Entities;

public class Servico
{
    public int Id { get; set; }
    public int GaragemId { get; set; }
    public Garagem? Garagem { get; set; }
    public int TipoServicoId { get; set; }
    public TipoServico? TipoServico { get; set; }
    public string Nome { get; set; } = null!;
    public string? Descricao { get; set; }
    public int? TempoEstimado { get; set; }
    public bool Ativo { get; set; }
    public ICollection<OrdemServicoServico> OrdensServico { get; set; } = new List<OrdemServicoServico>();
}