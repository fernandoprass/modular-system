using Shared.Domain.Entities;
using Shared.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Shared.Infrastructure.UoW;

public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
{
    private readonly TContext _dbContext;
    private readonly IUserContext _userContext;

    public UnitOfWork(TContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var userId = _userContext.UserId;
        var entries = _dbContext.ChangeTracker.Entries<Entity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;

                // For the case when creating the first user (or system user), 
                // the CreatedBy should be the user itself if no context user exists.
                // We assume there's a convention or specific check for "User" type if needed, 
                // but usually, it's safer to just check if userId is empty.
                entry.Entity.CreatedBy = userId == Guid.Empty ? entry.Entity.CreatedBy : userId;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedBy = _userContext.UserId == Guid.Empty ? null : _userContext.UserId;
            }
        }
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
