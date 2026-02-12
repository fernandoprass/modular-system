using IAM.Domain.DTOs.Requests;
using Myce.Response;
using Myce.Validation;

namespace IAM.Application.Validators.User
{
   internal class CreateCustometerValidator 
   {
      public Result Validate(CreateCustomerRequest request)
      {
         var validator = new EntityValidator<CreateCustomerRequest>()
             .RuleFor(x => x.CustomerName).IsRequired().MinLength(3).Apply()
             .RuleFor(x => x.CustomerCode).IsRequired().MinLength(3).Apply()
             .RuleFor(x => x.UserEmail).IsRequired().IsValidEmailAddress().Apply()
             .RuleFor(x => x.Password).IsRequired().MinLength(8).Apply();
             
         var isValid = validator.Validate(request);

         return isValid ? Result.Success() : Result.Failure(validator.Messages);
      }
   }
}
