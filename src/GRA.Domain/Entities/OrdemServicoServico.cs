using GRA.Domain.Entities.Commom;

namespace GRA.Domain.Entities;

public class OrdemServicoServico : Entity
{
    public long OficinaId { get; set; }
    public Oficina? Oficina { get; set; }

    public long OrdemServicoId { get; set; }
    public OrdemServico? OrdemServico { get; set; }

    public long ServicoId { get; set; }
    public Servico? Servico { get; set; }

    public string? Observacoes { get; set; }
}
