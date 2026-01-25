using IAM.Domain.Entities;

namespace IAM.Domain.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user);
    void Update(User user);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}