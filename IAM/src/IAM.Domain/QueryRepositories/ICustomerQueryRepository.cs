using IAM.Domain.DTOs.Responses;

namespace IAM.Domain.QueryRepositories;

public interface ICustomerQueryRepository
{
    Task<CustomerDto?> GetByIdAsync(Guid id);
    Task<CustomerDto?> GetByNameAsync(string name);
    Task<IEnumerable<CustomerDto>> GetAllAsync();
}