using GRA.Domain.Entities.Commom;

namespace GRA.Domain.Entities;

public class TipoServico : Entity
{
    public int GaragemId { get; set; }
    public Garagem? Garagem { get; set; }

    public required string Nome { get; set; }
    public string? Descricao { get; set; }
    public bool Ativo { get; set; } = true;

    public ICollection<Servico> Servicos { get; set; } = [];
}
