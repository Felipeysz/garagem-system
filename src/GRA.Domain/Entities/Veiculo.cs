namespace GRA.Domain.Entities;

public class Veiculo
{
    public int Id { get; set; }
    public int GaragemId { get; set; }
    public Garagem? Garagem { get; set; }
    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public string Placa { get; set; } = null!;
    public string? Chassi { get; set; }
    public string Marca { get; set; } = null!;
    public string Modelo { get; set; } = null!;
    public int Ano { get; set; }
    public string? Cor { get; set; }
    public int Quilometragem { get; set; }
    public string? Observacoes { get; set; }
    public bool Ativo { get; set; }
    public ICollection<OrdemServico> OrdensServico { get; set; } = new List<OrdemServico>();
}