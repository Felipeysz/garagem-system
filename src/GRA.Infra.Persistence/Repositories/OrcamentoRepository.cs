using GRA.Domain.Entities;
using GRA.Domain.Repositories;
using GRA.Infra.Persistence.Context;
using GRA.Infra.Persistence.Repositories;

public class OrcamentoRepository : Repository<Orcamento>, IOrcamentoRepository
{
    public OrcamentoRepository(GRAContext context) : base(context)
    {
    }
}