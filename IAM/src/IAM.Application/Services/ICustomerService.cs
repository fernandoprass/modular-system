using IAM.Domain.DTOs.Responses;
using IAM.Domain.Entities;

namespace IAM.Application.Services;

public interface ICustomerService
{
    Task<CustomerDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<CustomerDto>> GetAllAsync();
    Task<CustomerDto?> GetByNameAsync(string name);
    Task<Customer> CreateAsync(CustomerCreateRequest customerCreateRequest);
    Task UpdateAsync(Customer customer);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}