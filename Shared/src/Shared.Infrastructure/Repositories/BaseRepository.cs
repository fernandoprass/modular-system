using Microsoft.EntityFrameworkCore;
using Shared.Domain.Entities;
using Shared.Domain.Interfaces;

namespace Shared.Infrastructure.Repositories;

/// <summary>
/// The BaseRepository class provides a generic implementation of the IRepository interface for basic CRUD operations on entities. 
/// It uses Entity Framework Core's DbContext and DbSet to interact with the database. This class can be inherited by specific 
/// repositories for different entities, allowing for code reuse and consistency across the data access layer.
/// </summary>
/// <typeparam name="T">The Type of the Entity</typeparam>
/// <typeparam name="TId">The Type of the Entity Id</typeparam>
/// <param name="context">The DbContext instance used for database operations</param>
public abstract class BaseRepository<T, TId>(DbContext context) : IBaseRepository<T, TId>
    where T : Entity
{
   protected readonly DbContext _context = context;
   protected readonly DbSet<T> _dbSet = context.Set<T>();

   public virtual async Task<T?> GetByIdAsync(TId id)
   {
      return await _dbSet.FindAsync(id);
   }

   public virtual Task AddAsync(T entity)
   {
      return _dbSet.AddAsync(entity).AsTask();
   }

   public virtual void Update(T entity)
   {
      _dbSet.Update(entity);
   }

   public virtual async Task DeleteAsync(TId id)
   {
      var entity = await _dbSet.FindAsync(id);
      if (entity != null)
      {
         _dbSet.Remove(entity);
      }
   }

   public virtual async Task<bool> ExistsAsync(TId id)
   {
      var entity = await _dbSet.FindAsync(id);
      return entity != null;
   }
}

/// <summary>
/// The BaseRepository class is a specialization of the generic BaseRepository for entities that use Guid as their identifier type.
/// </summary>
/// <typeparam name="T">The Type of the Entity</typeparam>
/// <param name="context">The DbContext instance used for database operations</param>
public abstract class BaseRepository<T>(DbContext context) 
   : BaseRepository<T, Guid>(context) where T : Entity
{

}