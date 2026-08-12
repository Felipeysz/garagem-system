namespace GRA.Domain.Entities;

public class OrdemServico
{
    public int Id { get; set; }

    public int GaragemId { get; set; }
    public Garagem? Garagem { get; set; }

    public int VeiculoId { get; set; }
    public Veiculo? Veiculo { get; set; }

    public int FuncionarioResponsavelId { get; set; }
    public Funcionario? FuncionarioResponsavel { get; set; }

    public DateTime DataAbertura { get; set; } = DateTime.UtcNow;
    public DateTime? DataFinalizacao { get; set; }
    public int QuilometragemEntrada { get; set; }

    /// <summary>
    /// Mantido como string (igual ao schema, NVARCHAR(50)) por não haver um CHECK
    /// listando os valores possíveis. Se o fluxo de status for fechado, considerar
    /// migrar para enum futuramente.
    /// </summary>
    public required string Status { get; set; }
    public string? Observacoes { get; set; }

    public Orcamento? Orcamento { get; set; }
    public ICollection<OrdemServicoServico> Servicos { get; set; } = [];
    public ICollection<MovimentacaoEstoque> Movimentacoes { get; set; } = [];
}
