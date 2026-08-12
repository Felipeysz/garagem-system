namespace GRA.Domain.Entities;

public class Veiculo
{
    public int Id { get; set; }

    public int GaragemId { get; set; }
    public Garagem? Garagem { get; set; }

    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public required string Placa { get; set; }
    public string? Chassi { get; set; }
    public required string Marca { get; set; }
    public required string Modelo { get; set; }
    public int Ano { get; set; }
    public string? Cor { get; set; }
    public int Quilometragem { get; set; }
    public string? Observacoes { get; set; }
    public bool Ativo { get; set; } = true;

    public ICollection<OrdemServico> OrdensServico { get; set; } = [];
}
