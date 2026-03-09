using IAM.Domain.Entities;

namespace IAM.Domain.Repositories;

public interface ICustomerRepository
{
    Task AddAsync(Customer customer);
    void Update(Customer customer);
    Task DeleteAsync(Guid id);
    Task<Customer?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}