using GRA.Domain.Entities.Commom;

namespace GRA.Domain.Entities;

public class Funcionario : Entity
{
    public long OficinaId { get; set; }
    public Oficina? Oficina { get; set; }

    public required string Nome { get; set; }
    public required string SenhaHash { get; set; }
    public required string CPF { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public required string Cargo { get; set; }
    public DateOnly DataAdmissao { get; set; }
    public bool Ativo { get; set; } = true;

    public ICollection<OrdemServico> OrdensServicoResponsavel { get; set; } = [];
}
