using Myce.FluentValidator;
using Myce.Response;

namespace IAM.Application.Validators.User
{
   internal class CreateCustometerValidator 
   {
      public Result Validate(CustomerCreateRequest request)
      {
         var validator = new FluentValidator<CustomerCreateRequest>()
             .RuleFor(x => x.Name).IsRequired().MinLength(3)
             .RuleFor(x => x.Code).IsRequired().MinLength(3);

         //todo: validate user
         var isValid = validator.Validate(request);

         return isValid ? Result.Success() : Result.Failure(validator.Messages);
      }
   }
}
