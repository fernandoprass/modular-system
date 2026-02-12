using IAM.Domain.DTOs.Responses;
using IAM.Domain.Entities;
using IAM.Domain.Repositories;
using IAM.Domain.QueryRepositories;

namespace IAM.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomerQueryRepository _customerQueryRepository;

    public CustomerService(
        IUnitOfWork unitOfWork,
        ICustomerQueryRepository customerQueryRepository)
    {
        _unitOfWork = unitOfWork;
        _customerQueryRepository = customerQueryRepository;
    }

    public async Task<CustomerDto?> GetByIdAsync(Guid id)
    {
        return await _customerQueryRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync()
    {
        return await _customerQueryRepository.GetAllAsync();
    }

    public async Task<CustomerDto?> GetByNameAsync(string name)
    {
        return await _customerQueryRepository.GetByNameAsync(name);
    }

    public async Task<Customer> CreateAsync(CustomerCreateRequest customerCreate)
    {
        var customer = new Customer
        {
            Id = Guid.CreateVersion7(),
            Type = customerCreate.Type,
            Name = customerCreate.Type == CustomerType.Company 
                   ? customerCreate.CustomerName 
                   : customerCreate.UserName,
            Code = customerCreate.CustomerCode,
            CreatedAt = DateTime.UtcNow
        };

        var user = new User
        {
            Id = Guid.CreateVersion7(),
            Name = customerCreate.UserName,
            Email = customerCreate.UserEmail,
            PasswordHash = Argon2.Hash(customerCreate.UserPassword),
            CustomerId = customer.Id,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Customers.AddAsync(customer);
        await _unitOfWork.Customers.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return customer;
    }

    public async Task UpdateAsync(Customer customer)
    {
        customer.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Customers.Update(customer);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _unitOfWork.Customers.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _unitOfWork.Customers.ExistsAsync(id);
    }
}