using GRA.Domain.Entities;
using GRA.Domain.Repositories;
using GRA.Infra.Persistence.Context;
using GRA.Infra.Persistence.Repositories;

public class FuncionarioRepository : RepositoryBase<Funcionario>, IFuncionarioRepository
{
    public FuncionarioRepository(GRAContext context) : base(context)
    {
    }
}