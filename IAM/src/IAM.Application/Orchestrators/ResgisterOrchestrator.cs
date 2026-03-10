using IAM.Application.Contracts;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;
using IAM.Domain.QueryRepositories;
using Myce.Response;

namespace IAM.Application.Orchestrators
{
   public class ResgisterOrchestrator : IRegisterOrchestrator
   {
      private readonly IUserService _userService;
      private readonly ICustomerQueryRepository _customerQueryRepository;
      private readonly IUserQueryRepository _userQueryRepository;

      public ResgisterOrchestrator(
         IUserService userService,
         ICustomerQueryRepository customerQueryRepository,
         IUserQueryRepository userQueryRepository,
         IUserValidator userValidator)
      {
         _userService = userService;
         _customerQueryRepository = customerQueryRepository;
         _userQueryRepository = userQueryRepository;
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
   }
}
