using GRA.Domain.Entities.Commom;

namespace GRA.Domain.Entities;

public class OrdemServicoServico : Entity
{
    public int OficinaId { get; set; }
    public Oficina? Oficina { get; set; }

    public int OrdemServicoId { get; set; }
    public OrdemServico? OrdemServico { get; set; }

    public int ServicoId { get; set; }
    public Servico? Servico { get; set; }

    public string? Observacoes { get; set; }
}
