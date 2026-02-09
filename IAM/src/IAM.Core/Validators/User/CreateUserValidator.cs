using IAM.Domain.DTOs.Requests;
using Myce.Response;
using Myce.Validation;

namespace IAM.Core.Validators.User
{
   internal class CreateUserValidator 
   {
      public Result Validate(CreateUserRequest request)
      {
         var validator = new EntityValidator<CreateUserRequest>()
             .RuleFor(x => x.Name).IsRequired().MinLength(3).Apply()
             .RuleFor(x => x.Email).IsRequired().IsValidEmailAddress().Apply()
             .RuleFor(x => x.Password).IsRequired().MinLength(8).Apply()
             .RuleFor(x => x.CustomerId).IsRequired().Apply();

         var isValid = validator.Validate(request);

         return isValid ? Result.Success() : Result.Failure(validator.Messages);

      }
   }
}
