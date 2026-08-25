using System.Linq.Expressions;
using GRA.Domain.Entities.Commom;
using GRA.Domain.Repositories;
using GRA.Infra.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace GRA.Infra.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : Entity
{
    protected readonly GRAContext Context;
    protected readonly DbSet<T> DbSet;

    public Repository(GRAContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public virtual Task<T?> GetByIdAsync(long id)
        => DbSet.FindAsync(id).AsTask();

    public virtual async Task<IEnumerable<T>> GetAllAsync()
        => await DbSet.ToListAsync();

    public virtual Task<T?> SingleAsync(Expression<Func<T, bool>> predicate)
        => DbSet.SingleOrDefaultAsync(predicate);

    public async Task<IEnumerable<T>> SearchAsync(Expression<Func<T, bool>> predicate)
        => await DbSet.Where(predicate).ToListAsync();

    public Task<bool> ExisteAsync(Expression<Func<T, bool>> predicate)
        => DbSet.AnyAsync(predicate);

    public Task AddAsync(T entity)
        => DbSet.AddAsync(entity).AsTask();

    public void Update(T entity)
        => DbSet.Update(entity);

    public void Remove(T entity)
        => DbSet.Remove(entity);

    public async Task<bool> SaveChangesAsync()
        => await Context.SaveChangesAsync() > 0;
}