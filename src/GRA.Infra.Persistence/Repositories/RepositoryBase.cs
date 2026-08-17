// GRA.Infra.Persistence/Repositories/Repository.cs
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

    public async Task<T?> GetByIdAsync(long id)
        => await DbSet.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync()
        => await DbSet.ToListAsync();

    public async Task<T?> SingleAsync(Expression<Func<T, bool>> predicate)
        => await DbSet.SingleOrDefaultAsync(predicate);

    public async Task<IEnumerable<T>> SearchAsync(Expression<Func<T, bool>> predicate)
        => await DbSet.Where(predicate).ToListAsync();

    public async Task AddAsync(T entity)
        => await DbSet.AddAsync(entity);

    public void Update(T entity)
        => DbSet.Update(entity);

    public void Remove(T entity)
        => DbSet.Remove(entity);

    public async Task<bool> SaveChangesAsync()
        => await Context.SaveChangesAsync() > 0;
}