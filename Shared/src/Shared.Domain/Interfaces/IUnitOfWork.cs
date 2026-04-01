using Microsoft.EntityFrameworkCore;

namespace Shared.Domain.Interfaces;

public interface IUnitOfWork<TContext> : IDisposable where TContext : DbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
