using IAM.Application.Contracts;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;
using IAM.Domain.Messages.Errors;
using IAM.Domain.Messages.Info;
using IAM.Domain.QueryRepositories;
using IAM.Domain.Repositories;
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

        customer.Update(request.Code, request.Name, request.Description, request.IsActive);
        
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