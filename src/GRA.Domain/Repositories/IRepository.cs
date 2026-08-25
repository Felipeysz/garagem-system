using System.Linq.Expressions;
using GRA.Domain.Entities.Commom;

namespace GRA.Domain.Repositories;

public interface IRepository<T> where T : Entity
{
    Task<T?> GetByIdAsync(long id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> SingleAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> SearchAsync(Expression<Func<T, bool>> predicate);
    Task<bool> ExisteAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
    Task<bool> SaveChangesAsync();
}