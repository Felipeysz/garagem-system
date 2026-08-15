namespace GRA.Domain.Entities;

public class Funcionario
{
    public int Id { get; set; }
    public int GaragemId { get; set; }
    public Garagem? Garagem { get; set; }
    public string Nome { get; set; } = null!;
    public string CPF { get; set; } = null!;
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string Cargo { get; set; } = null!;
    public DateOnly DataAdmissao { get; set; }
    public bool Ativo { get; set; }
    public ICollection<OrdemServico> OrdensServicoResponsavel { get; set; } = new List<OrdemServico>();
}