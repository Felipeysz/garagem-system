using GRA.Domain.Entities;
using GRA.Domain.Repositories;
using GRA.Infra.Persistence.Context;

namespace GRA.Infra.Persistence.Repositories;

public class OficinaRepository : RepositoryBase<Oficina>, IOficinaRepository
{
    public OficinaRepository(GRAContext context) : base(context)
    {
    }
}