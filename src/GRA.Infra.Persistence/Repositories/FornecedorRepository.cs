using GRA.Domain.Entities;
using GRA.Domain.Repositories;
using GRA.Infra.Persistence.Context;
using GRA.Infra.Persistence.Repositories;

public class FornecedorRepository : RepositoryBase<Fornecedor>, IFornecedorRepository
{
    public FornecedorRepository(GRAContext context) : base(context)
    {
    }
}