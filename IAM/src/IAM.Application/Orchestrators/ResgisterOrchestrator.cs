using IAM.Application.Contracts;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;
using IAM.Domain.Entities;
using IAM.Domain.Mappers;
using IAM.Domain.Messages.Errors;
using IAM.Domain.QueryRepositories;
using IAM.Domain.Repositories;
using Isopoh.Cryptography.Argon2;
using Myce.Response;

namespace IAM.Application.Orchestrators
{
   public class ResgisterOrchestrator : IRegisterOrchestrator
   {
      private readonly IUserService _userService;
      private readonly ICustomerQueryRepository _customerQueryRepository;
      private readonly IUserQueryRepository _userQueryRepository;
      private readonly IUnitOfWork _unitOfWork;
      private readonly ICustomerValidator _customerValidator;

      public ResgisterOrchestrator(
         IUserService userService,
         ICustomerQueryRepository customerQueryRepository,
         IUserQueryRepository userQueryRepository,
         IUnitOfWork unitOfWork,
         ICustomerValidator customerValidator)
      {
         _userService = userService;
         _customerQueryRepository = customerQueryRepository;
         _userQueryRepository = userQueryRepository;
         _unitOfWork = unitOfWork;
         _customerValidator = customerValidator;
      }

      public async Task<Result<UserDto>> RegisterUserAsync(UserCreateRequest request, Guid operatorCustomerId)
      {
         var userId = await _userQueryRepository.GetIdByEmailAsync(request.Email);
         var customerDto = await _customerQueryRepository.GetByIdAsync(request.CustomerId);

         var emailExists =  userId != Guid.Empty;
         var customerExists = customerDto is not null;

         var result = await _userService.CreateUserAsync(request, operatorCustomerId, customerExists, emailExists);

         if (result.IsValid) { 
            result.Data.CustomerName = customerDto.Name; 
         }
         
         return result;
      }
      public async Task<Result<CustomerDto>> RegisterCustomerAsync(CustomerCreateRequest customerCreate)
      {
         //todo generate a random code for customer if type is person, and validate it doesn't exist in database
         var validation = _customerValidator.ValidateCreate(customerCreate);
         if (validation.HasError)
         {
            return Result<CustomerDto>.Failure(validation.Messages);
         }

         if (customerCreate.Type == CustomerType.Company)
         {
            var codeExists = await _customerQueryRepository.ExistsByCodeAsync(customerCreate.Code);
            if (codeExists)
            {
               return Result<CustomerDto>.Failure(new DuplicateCodeError(customerCreate.Code));
            }
         }

         var customer = Customer.Create(
            customerCreate.Type,
            customerCreate.Code,
            customerCreate.Type.Equals(CustomerType.Company) ? customerCreate.Name : customerCreate.User.Name,
            customerCreate.Description
         );

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
