namespace GRA.Domain.Entities;

public class Cliente
{
    public int Id { get; set; }
    public int GaragemId { get; set; }
    public Garagem? Garagem { get; set; }
    public string Nome { get; set; } = null!;
    public string CPF { get; set; } = null!;
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public DateTime DataCadastro { get; set; }
    public bool Ativo { get; set; }
    public ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
}