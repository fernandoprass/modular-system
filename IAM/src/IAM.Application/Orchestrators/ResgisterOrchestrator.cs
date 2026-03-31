using IAM.Application.Contracts;
using IAM.Application.Services;
using IAM.Domain;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;
using IAM.Domain.Entities;
using IAM.Domain.Enums;
using IAM.Domain.Interfaces;
using IAM.Domain.Mappers;
using IAM.Domain.Messages.Errors;
using IAM.Domain.Messages.Info;
using IAM.Domain.QueryRepositories;
using IAM.Domain.Repositories;
using Isopoh.Cryptography.Argon2;
using Myce.Response;

namespace IAM.Application.Orchestrators
{
   public class ResgisterOrchestrator(
      ICustomerService customerService,
      ICustomerQueryRepository customerQueryRepository,
      IUserContext userContext,
      IUserRepository userRepository,
      IUserService userService,
      IUnitOfWork unitOfWork) : BaseService(userContext), IRegisterOrchestrator
   {
      private readonly ICustomerService _customerService = customerService;
      private readonly ICustomerQueryRepository _customerQueryRepository = customerQueryRepository;
      private readonly IUserRepository _userRepository = userRepository;
      private readonly IUserService _userService = userService;
      private readonly IUnitOfWork _unitOfWork = unitOfWork;

      public async Task<Result<UserDto>> RegisterUserAsync(UserCreateRequest request)
      {
         var customerDto = await _customerQueryRepository.GetByIdAsync(request.CustomerId);

         var customerExists = customerDto is not null;

         var result = await _userService.CreateUserAsync(request, customerExists);

         if (result.IsSuccess) { 
            result.Data.CustomerName = customerDto.Name; 
         }
         
         return result;
      }
      public async Task<Result<CustomerDto>> RegisterCustomerAsync(CustomerCreateRequest customerCreate)
      {   
         var customerValidateResult = await _customerService.ValidateCreateCustomerAsync(customerCreate);
         var userValidateResult = await _userService.ValidateUserForNewCustomerAsync(customerCreate.User);

         var result = Result.Merge(customerValidateResult, userValidateResult);

         var customer = Customer.Create(
            customerCreate.Type,
            customerCreate.Type.Equals(CustomerType.Company) ? customerCreate.Code : _customerService.GetRandomCode(),
            customerCreate.Type.Equals(CustomerType.Company) ? customerCreate.Name : customerCreate.User.Name,
            customerCreate.Description
         );

         if (result.HasError) { 
            return Result<CustomerDto>.Failure(result.Messages); 
         }

         var user = User.Create(
          customerCreate.User.Name,
          customerCreate.User.Email,
          Argon2.Hash(customerCreate.User.Password),
          customer.Id
         );

         customer.CreatedBy = user.Id;

         await _unitOfWork.Customers.AddAsync(customer);
         await _unitOfWork.Users.AddAsync(user);
         await _unitOfWork.SaveChangesAsync();

         return Result<CustomerDto>.Success(customer.ToCustomerDto());
      }

      public async Task<Result> DeleteCustomerAsync(Guid id)
      {
         return await ExecuteIfUserOwnsAsync(id, async () =>
         {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null)
            {
               return Result.Failure(new NotFoundError(Const.Entity.Customer));
            }

            await _unitOfWork.Customers.DeleteAsync(id);

            var users = await _userRepository.GetByCustomerIdAsync(id);
            foreach(var user in users)
            {
               await _unitOfWork.Users.DeleteAsync(user.Id);
            }

            await _unitOfWork.SaveChangesAsync();

            return Result.Success(new SuccessInfo());
         });
      }
   }
}
