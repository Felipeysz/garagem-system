using GRA.Domain.Entities;
using GRA.Domain.Repositories;
using GRA.Infra.Persistence.Context;
using GRA.Infra.Persistence.Repositories;

public class OrdemServicoRepository : Repository<OrdemServico>, IOrdemServicoRepository
{
    public OrdemServicoRepository(GRAContext context) : base(context)
    {
    }
}