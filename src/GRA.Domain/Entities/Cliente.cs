namespace GRA.Domain.Entities;

public class Cliente
{
    public int Id { get; set; }

    public int GaragemId { get; set; }
    public Garagem? Garagem { get; set; }

    public required string Nome { get; set; }
    public required string CPF { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    public bool Ativo { get; set; } = true;

    public ICollection<Veiculo> Veiculos { get; set; } = [];
}
