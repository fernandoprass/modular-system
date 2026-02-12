using IAM.Domain.DTOs.Requests;
using Myce.Response;
using Myce.Validation;

namespace IAM.Application.Validators.User
{
   internal class CreateUserValidator 
   {
      private readonly IUserQueryRepository _userQueryRepository;

      public CreateUserValidator(IUserQueryRepository userQueryRepository)
      {
         _userQueryRepository = userQueryRepository;
      }
      public Result Validate(CreateUserRequest request)
      {
         var emailExists = _userQueryRepository.GetIdByEmailAsync(request.Email);

         var validator = new EntityValidator<CreateUserRequest>()
             .RuleFor(x => x.Name).IsRequired().MinLength(3).Apply()
             .RuleFor(x => x.Email).IsRequired().IsValidEmailAddress().IsEqualTo(false).Apply()
             .RuleFor(x => x.Password).IsRequired().MinLength(8).Apply()
             .RuleFor(x => x.CustomerId).IsRequired().Apply();

         var isValid = validator.Validate(request);

         return isValid ? Result.Success() : Result.Failure(validator.Messages);
      }
   }
}
