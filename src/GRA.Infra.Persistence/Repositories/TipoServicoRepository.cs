using GRA.Domain.Entities;
using GRA.Domain.Repositories;
using GRA.Infra.Persistence.Context;
using GRA.Infra.Persistence.Repositories;

public class TipoServicoRepository : Repository<TipoServico>, ITipoServicoRepository
{
    public TipoServicoRepository(GRAContext context) : base(context)
    {
    }
}