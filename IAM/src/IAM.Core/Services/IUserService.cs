using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;
using IAM.Domain.Entities;

namespace IAM.Core.Services;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto?> GetByEmailAsync(string email);
    Task<IEnumerable<UserDto>> GetByCustomerIdAsync(Guid customerId);
    Task<User> CreateAsync(User user);
    Task<User> CreateUserAsync(CreateUserRequest request);
    Task<UserDto?> ValidateCredentialsAsync(string email, string password);
    Task UpdateAsync(User user);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}