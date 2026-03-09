using IAM.Application.Contracts;
using IAM.Domain.DTOs.Requests;
using Myce.FluentValidator;
using Myce.Response;

namespace IAM.Application.Validators;

public class CustomerValidator : ICustomerValidator
{
   public Result ValidateCreate(CustomerCreateRequest request)
   {
      var validator = new FluentValidator<CustomerCreateRequest>()
          .RuleFor(x => x.Name).IsRequired().MinLength(3)
          .RuleFor(x => x.Code).IsRequired().MinLength(3)
          .RuleFor(x => x.User.Name).IsRequired().MinLength(3)
          .RuleFor(x => x.User.Email).IsRequired().IsValidEmailAddress()
          .RuleFor(x => x.User.Password).IsRequired().MinLength(8);

      var isValid = validator.Validate(request);

      return isValid ? Result.Success() : Result.Failure(validator.Messages);
   }

   public Result ValidateUpdate(Guid id, CustomerUpdateRequest request)
   {
      var validator = new FluentValidator<CustomerUpdateRequest>()
          .RuleFor(x => x.Name).IsRequired().MinLength(3)
          .RuleFor(x => x.Code).IsRequired().MinLength(3);

      var isValid = validator.Validate(request);

      return isValid ? Result.Success() : Result.Failure(validator.Messages);
   }
}
