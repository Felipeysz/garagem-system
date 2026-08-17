using GRA.Domain.Entities;
using GRA.Domain.Repositories;
using GRA.Infra.Persistence.Context;
using GRA.Infra.Persistence.Repositories;

public class PecaRepository : RepositoryBase<Peca>, IPecaRepository
{
    public PecaRepository(GRAContext context) : base(context)
    {
    }
}