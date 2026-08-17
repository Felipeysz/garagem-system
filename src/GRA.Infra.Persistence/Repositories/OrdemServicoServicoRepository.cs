using GRA.Domain.Entities;
using GRA.Domain.Repositories;
using GRA.Infra.Persistence.Context;
using GRA.Infra.Persistence.Repositories;

public class OrdemServicoServicoRepository : RepositoryBase<OrdemServicoServico>, IOrdemServicoServicoRepository
{
    public OrdemServicoServicoRepository(GRAContext context) : base(context)
    {
    }
}