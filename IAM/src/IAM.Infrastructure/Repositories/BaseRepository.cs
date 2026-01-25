using Microsoft.EntityFrameworkCore;

namespace IAM.Infrastructure.Repositories;

public abstract class BaseRepository<T> where T : class
{
    protected readonly IamDbContext _context;
    protected readonly DbSet<T> _dbSet;

    protected BaseRepository(IamDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual Task AddAsync(T entity)
    {
        return _dbSet.AddAsync(entity).AsTask();
    }

    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public virtual async Task<bool> ExistsAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        return entity != null;
    }
}