using GRA.Domain.Enums;

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
    public DateTime DataAbertura { get; set; }
    public DateTime? DataFinalizacao { get; set; }
    public int QuilometragemEntrada { get; set; }
    public StatusOrdemServico Status { get; set; }
    public string? Observacoes { get; set; }
    public Orcamento? Orcamento { get; set; }
    public ICollection<OrdemServicoServico> Servicos { get; set; } = new List<OrdemServicoServico>();
    public ICollection<MovimentacaoEstoque> Movimentacoes { get; set; } = new List<MovimentacaoEstoque>();
}