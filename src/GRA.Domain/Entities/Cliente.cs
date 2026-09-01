using GRA.Domain.Entities.Commom;

namespace GRA.Domain.Entities;

public class Cliente : Entity
{
    public required string Nome { get; set; }
    public required string SenhaHash { get; set; }
    public required string CPF { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    public bool Ativo { get; set; } = true;

    public ICollection<Veiculo> Veiculos { get; set; } = [];
}