using GRA.Domain.Entities.Commom;

namespace GRA.Domain.Entities;

public class Orcamento : Entity
{
    public int OficinaId { get; set; }
    public Oficina? Oficina { get; set; }

    public int OrdemServicoId { get; set; }
    public OrdemServico? OrdemServico { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime? DataAprovacao { get; set; }

    /// <summary>
    /// Mantido como string (igual ao schema, NVARCHAR(50)) por não haver um CHECK
    /// listando os valores possíveis. Se o fluxo de status for fechado, considerar
    /// migrar para enum futuramente.
    /// </summary>
    public required string Status { get; set; }
    public string? Observacoes { get; set; }
}
