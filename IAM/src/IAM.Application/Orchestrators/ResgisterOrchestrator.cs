using IAM.Application.Contracts;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;
using IAM.Domain.Entities;
using IAM.Domain.Mappers;
using IAM.Domain.QueryRepositories;
using IAM.Domain.Repositories;
using Isopoh.Cryptography.Argon2;
using Myce.Response;

namespace IAM.Application.Orchestrators
{
   public class ResgisterOrchestrator : IRegisterOrchestrator
   {
      private readonly ICustomerService _customerService;
      private readonly ICustomerQueryRepository _customerQueryRepository;
      private readonly IUserService _userService;
      private readonly IUserQueryRepository _userQueryRepository;
      private readonly IUnitOfWork _unitOfWork;
      

      public ResgisterOrchestrator(
         ICustomerService customerService,
         ICustomerQueryRepository customerQueryRepository,
         IUserService userService,
         IUserQueryRepository userQueryRepository,
         IUnitOfWork unitOfWork)
      {
         _customerService = customerService;
         _customerQueryRepository = customerQueryRepository;
         _userService = userService;
         _userQueryRepository = userQueryRepository;
         _unitOfWork = unitOfWork;  
      }

      public async Task<Result<UserDto>> RegisterUserAsync(UserCreateRequest request, Guid operatorCustomerId)
      {
         var customerDto = await _customerQueryRepository.GetByIdAsync(request.CustomerId);

         var customerExists = customerDto is not null;

         var result = await _userService.CreateUserAsync(request, operatorCustomerId, customerExists);

         if (result.IsValid) { 
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

         await _unitOfWork.Customers.AddAsync(customer);
         await _unitOfWork.Users.AddAsync(user);
         await _unitOfWork.SaveChangesAsync();

         return Result<CustomerDto>.Success(customer.ToCustomerDto());
      }
   }
}
