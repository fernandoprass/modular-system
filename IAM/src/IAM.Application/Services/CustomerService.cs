using IAM.Application.Contracts;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;
using IAM.Domain.Entities;
using IAM.Domain.QueryRepositories;
using IAM.Domain.Repositories;
using IAM.Domain.Mappers;
using IAM.Domain.Messages.Errors;
using IAM.Domain.Messages.Info;
using Isopoh.Cryptography.Argon2;
using Myce.Response;

namespace IAM.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomerQueryRepository _customerQueryRepository;
    private readonly ICustomerValidator _customerValidator;

    public CustomerService(
        IUnitOfWork unitOfWork,
        ICustomerQueryRepository customerQueryRepository,
        ICustomerValidator customerValidator)
    {
        _unitOfWork = unitOfWork;
        _customerQueryRepository = customerQueryRepository;
        _customerValidator = customerValidator;
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

    public async Task<Result<CustomerDto>> CreateAsync(CustomerCreateRequest customerCreate)
    {
        var validation = _customerValidator.ValidateCreate(customerCreate);
        if (validation.HasError)
        {
            return Result<CustomerDto>.Failure(validation.Messages);
        }

        var customer = new Customer
        {
            Id = Guid.CreateVersion7(),
            Type = customerCreate.Type,
            Name = customerCreate.Type == CustomerType.Company ? customerCreate.Name : customerCreate.Name,
            Code = customerCreate.Code,
            CreatedAt = DateTime.UtcNow
        };

        var user = User.Create(
         customerCreate.User.Name,
         customerCreate.User.Email,
         Argon2.Hash(customerCreate.User.Password),
         customer.Id
        );

        await _unitOfWork.Customers.AddAsync(customer);
        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
        
        return Result<CustomerDto>.Success(customer.ToCustomerDto());
    }

    public async Task<Result> UpdateAsync(Guid id, CustomerUpdateRequest request)
    {
        var validation = _customerValidator.ValidateUpdate(id, request);
        if (validation.HasError)
        {
            return Result.Failure(validation.Messages);
        }

        var customer = await _unitOfWork.Customers.GetByIdAsync(id);
        if (customer == null)
        {
            return Result.Failure(new NotFoundError("Customer"));
        }

        customer.Update(request.Name, request.Code, request.Description, request.IsActive);
        
        _unitOfWork.Customers.Update(customer);
        await _unitOfWork.SaveChangesAsync();
        
        return Result.Success(new SuccessInfo());
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(id);
        if (customer == null)
        {
            return Result.Failure(new NotFoundError("Customer"));
        }

        await _unitOfWork.Customers.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
        
        return Result.Success(new SuccessInfo());
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _unitOfWork.Customers.ExistsAsync(id);
    }
}