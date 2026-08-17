using GRA.Domain.Entities;
using GRA.Domain.Repositories;
using GRA.Infra.Persistence.Context;
using GRA.Infra.Persistence.Repositories;

public class ServicoRepository : RepositoryBase<Servico>, IServicoRepository
{
    public ServicoRepository(GRAContext context) : base(context)
    {
    }
}