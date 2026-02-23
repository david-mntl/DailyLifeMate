using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DailyLifeMate.Domain.Persistence;

public interface IRepository<T> where T : IEntity
{
    /// <summary>
    /// Gets an entity by its ID, with optional related entities to include.
    /// </summary>
    /// <param name="id">id of the entity that is desired to be obtained</param>
    /// <param name="includes">optional related entities to include in the result</param>
    /// <returns></returns>
    Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Gets all entities, with optional related entities to include.
    /// </summary>
    /// <param name="includes">optional related entities to include in the result</param>
    /// <returns>A list of all the available entries of type T</returns>
    Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
    void Add(T entity);
    void Update(T entity);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<bool> ExistsAsync(Guid id);
    Task<int> CountAsync();
    Task SaveChangesAsync();
}