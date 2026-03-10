using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;
using IAM.Domain.Entities;

using Myce.Response;

namespace IAM.Application.Contracts;

public interface ICustomerService
{
    Task<CustomerDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<CustomerDto>> GetAllAsync();
    Task<CustomerDto?> GetByNameAsync(string name);
    Task<Result> UpdateAsync(Guid id, CustomerUpdateRequest request);
    Task<Result> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}