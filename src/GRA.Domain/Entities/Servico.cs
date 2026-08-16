using GRA.Domain.Entities.Commom;

namespace GRA.Domain.Entities;

public class Servico : Entity
{
    public int OficinaId { get; set; }
    public Oficina? Oficina { get; set; }

    public int TipoServicoId { get; set; }
    public TipoServico? TipoServico { get; set; }

    public required string Nome { get; set; }
    public string? Descricao { get; set; }
    public int? TempoEstimado { get; set; }
    public bool Ativo { get; set; } = true;

    public ICollection<OrdemServicoServico> OrdensServico { get; set; } = [];
}
