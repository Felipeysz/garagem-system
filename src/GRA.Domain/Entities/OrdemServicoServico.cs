namespace GRA.Domain.Entities;

public class OrdemServicoServico
{
    public int Id { get; set; }

    public int GaragemId { get; set; }
    public Garagem? Garagem { get; set; }

    public int OrdemServicoId { get; set; }
    public OrdemServico? OrdemServico { get; set; }

    public int ServicoId { get; set; }
    public Servico? Servico { get; set; }

    public string? Observacoes { get; set; }
}
