using GRA.Domain.Entities.Commom;

namespace GRA.Domain.Repositories;

public interface IRepository<T> where T : Entity
{
    Task<T?> GetByIdAsync(long id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
    Task<int> SaveChangesAsync();
}