using IAM.Application.Contracts;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;
using IAM.Domain.QueryRepositories;
using Myce.Response;

namespace IAM.Application.Orchestrators
{
   public class UserOrchestrator : IUserOrchestrator
   {
      private readonly IUserService _userService;
      private readonly ICustomerQueryRepository _customerQueryRepository;
      private readonly IUserQueryRepository _userQueryRepository;
      private readonly IUserValidator _userValidator;

      public UserOrchestrator(
         IUserService userService,
         ICustomerQueryRepository customerQueryRepository,
         IUserQueryRepository userQueryRepository,
         IUserValidator userValidator)
      {
         _userService = userService;
         _customerQueryRepository = customerQueryRepository;
         _userQueryRepository = userQueryRepository;
         _userValidator = userValidator;
      }

      public async Task<Result<UserDto>> RegisterUserAsync(UserCreateRequest request, Guid operatorCustomerId)
      {
         var emailIdTask = _userQueryRepository.GetIdByEmailAsync(request.Email);
         var customerTask = _customerQueryRepository.GetByIdAsync(request.CustomerId);

         await Task.WhenAll(emailIdTask, customerTask);

         var emailExists = await emailIdTask != Guid.Empty;
         var customer = await customerTask;

         var validation = _userValidator.ValidateCreate(request, emailExists, customer is not null);
         if (validation.HasError)
         {
            return Result<UserDto>.Failure(validation.Messages);
         }

         return await _userService.CreateUserAsync(request);
      }
   }
}
