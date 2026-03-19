using IAM.Application.Contracts;
using IAM.Domain;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;
using IAM.Domain.Messages;
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
   private readonly ICustomerRepository _customerRepository;
   private readonly ICustomerValidator _customerValidator;

   public CustomerService(
       IUnitOfWork unitOfWork,
       ICustomerQueryRepository customerQueryRepository,
       ICustomerRepository customerRepository,
       ICustomerValidator customerValidator)
   {
      _unitOfWork = unitOfWork;
      _customerQueryRepository = customerQueryRepository;
      _customerRepository = customerRepository;
      _customerValidator = customerValidator;
   }
   public async Task<Result> ValidateCreateCustomerAsync(CustomerCreateRequest request)
   {
      var codeExists = await _customerQueryRepository.ExistsByCodeAsync(request.Code);

      var validation = _customerValidator.ValidateCreate(request, codeExists);
      if (validation.HasError)
      {
         return Result.Failure(validation.Messages);
      }

      return Result.Success();
   }

   public async Task<CustomerDto?> GetByIdAsync(Guid id)
   {
      return await _customerQueryRepository.GetByIdAsync(id);
   }

   public string GetRandomCode()
   {
      const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
      var random = new Random();
      return new string(Enumerable.Repeat(chars, Const.Customer.RandomCodeSize)
          .Select(s => s[random.Next(s.Length)]).ToArray());
   }

   public async Task<IEnumerable<CustomerDto>> GetByNameAsync(string name)
   {
      return await _customerQueryRepository.GetByNameAsync(name);
   }

   public async Task<Result> UpdateAsync(Guid id, CustomerUpdateRequest request, Guid operatorCustomerId)
   {
      if (id != operatorCustomerId)
      {
         return Result.Failure(new ForbiddenCustomerError());
      }

      var validation = _customerValidator.ValidateUpdate(request);
      if (validation.HasError)
      {
         return Result.Failure(validation.Messages);
      }

      var customer = await _unitOfWork.Customers.GetByIdAsync(id);
      if (customer == null)
      {
         return Result.Failure(new NotFoundError(Const.Entity.Customer));
      }

      customer.Update(request.Name, request.Description, request.IsActive);

      _unitOfWork.Customers.Update(customer);
      await _unitOfWork.SaveChangesAsync();

      return Result.Success(new SuccessInfo());
   }

   public async Task<Result> UpdateCodeAsync(Guid id, CustomerUpdateCodeRequest request, Guid operatorCustomerId)
   {
      if (id != operatorCustomerId)
      {
         return Result.Failure(new ForbiddenCustomerError());
      }

      var customer = await _customerRepository.GetByCodeAsync(request.Code);
      var newCodeExists = customer is null;

      customer = await _customerRepository.GetByIdAsync(id);

      var validation = _customerValidator.ValidateUpdateCode(request, newCodeExists);
      if (validation.HasError)
      {
         return Result.Failure(validation.Messages);
      }

      customer.Update(request.Code);

      _unitOfWork.Customers.Update(customer);
      await _unitOfWork.SaveChangesAsync();

      return Result.Success(new SuccessInfo());
   }

   public async Task<Result> DeleteAsync(Guid id)
   {
      var customer = await _unitOfWork.Customers.GetByIdAsync(id);
      if (customer == null)
      {
         return Result.Failure(new NotFoundError(Const.Entity.Customer));
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