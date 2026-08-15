namespace GRA.Domain.Entities;

public class TipoServico
{
    public int Id { get; set; }
    public int GaragemId { get; set; }
    public Garagem? Garagem { get; set; }
    public string Nome { get; set; } = null!;
    public string? Descricao { get; set; }
    public bool Ativo { get; set; }
    public ICollection<Servico> Servicos { get; set; } = new List<Servico>();
}