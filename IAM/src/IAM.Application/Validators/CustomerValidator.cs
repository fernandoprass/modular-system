using IAM.Application.Contracts;
using IAM.Domain;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.Messages.Errors;
using Myce.FluentValidator;
using Myce.Response;

namespace IAM.Application.Validators;

public class CustomerValidator : ICustomerValidator
{
   private static void CodeRules<T>(RuleBuilder<T, string> rb) where T : class
                           => rb.IsRequired().MinLength(3).IsAlphaNumeric();

   public Result ValidateCreate(CustomerCreateRequest request, bool newCodeExists)
   {
      var validator = new FluentValidator<CustomerCreateRequest>()
          .RuleFor(x => x.Type).IsInEnum(new InvalidCustomerTypeError())
          .RuleFor(x => x.Name).ApplyTemplate(ValidatorTemplate.NameRules)
          .RuleFor(x => x.Code).If(x => x.Type.Equals(CustomerType.Company), x => x.ApplyTemplate(CodeRules))
          .RuleFor(x => x.User).IsNotNull() //just to ensure the user object is provided, validation is done in UserValidator
          .RuleForValue(newCodeExists).IsFalse(new DuplicateCustomerCodeError(request.Code));

      var isValid = validator.Validate(request);

      return isValid ? Result.Success() : Result.Failure(validator.Messages);
   }

   public Result ValidateUpdate(CustomerUpdateRequest request, bool customerExists)
   {
      var validator = new FluentValidator<CustomerUpdateRequest>()
          .RuleFor(x => x.Name).ApplyTemplate(ValidatorTemplate.NameRules)
          .RuleForValue(customerExists).IsTrue(new NotFoundError(Const.Entity.Customer));

      var isValid = validator.Validate(request);

      return isValid ? Result.Success() : Result.Failure(validator.Messages);
   }

   public Result ValidateUpdateCode(CustomerUpdateCodeRequest request, bool newCodeExists)
   {
      var validator = new FluentValidator<CustomerUpdateCodeRequest>()
          .RuleFor(x => x.Code).ApplyTemplate(CodeRules)
          .RuleForValue(newCodeExists).IsFalse(new DuplicateCustomerCodeError(request.Code));

      var isValid = validator.Validate(request);

      return isValid ? Result.Success() : Result.Failure(validator.Messages);
   }
}
