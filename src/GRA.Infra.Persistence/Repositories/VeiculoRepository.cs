using GRA.Domain.Entities;
using GRA.Domain.Repositories;
using GRA.Infra.Persistence.Context;
using GRA.Infra.Persistence.Repositories;

public class VeiculoRepository : RepositoryBase<Veiculo>, IVeiculoRepository
{
    public VeiculoRepository(GRAContext context) : base(context)
    {
    }
}