using System.Linq.Expressions;
using GRA.Domain.Entities;
using GRA.Domain.Repositories;
using GRA.Infra.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace GRA.Infra.Persistence.Repositories;

public class OficinaRepository : Repository<Oficina>, IOficinaRepository
{
    public OficinaRepository(GRAContext context) : base(context)
    {
    }

    public override async Task<Oficina?> GetByIdAsync(long id)
        => await Context.Oficinas
            .Include(o => o.Endereco)
            .FirstOrDefaultAsync(o => o.Id == id);

    public override async Task<IEnumerable<Oficina>> GetAllAsync()
        => await Context.Oficinas
            .Include(o => o.Endereco)
            .ToListAsync();

    public override async Task<Oficina?> SingleAsync(Expression<Func<Oficina, bool>> predicate)
        => await Context.Oficinas
            .Include(o => o.Endereco)
            .SingleOrDefaultAsync(predicate);
}