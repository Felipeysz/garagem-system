using GRA.Domain.Entities;
using GRA.Domain.Repositories;
using GRA.Infra.Persistence.Context;
using GRA.Infra.Persistence.Repositories;

public class ClienteRepository : RepositoryBase<Cliente>, IClienteRepository
{
    public ClienteRepository(GRAContext context) : base(context)
    {
    }
}