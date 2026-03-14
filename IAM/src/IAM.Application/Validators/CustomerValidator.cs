using IAM.Application.Contracts;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.Messages.Errors;
using Myce.FluentValidator;
using Myce.Response;

namespace IAM.Application.Validators;

public class CustomerValidator : ICustomerValidator
{
   private static void CodeRules<T>(RuleBuilder<T, string> rb) where T : class
                           => rb.IsRequired().MinLength(3).IsAlphaNumeric();

   public Result ValidateCreate(CustomerCreateRequest request, bool codeExists)
   {
      //todo use especif RuleFor when implemented in Myce
      bool isValidType = Enum.IsDefined(typeof(CustomerType), request.Type);

      var validator = new FluentValidator<CustomerCreateRequest>()
          .RuleFor(x => x.Type).Custom(isValidType, new InvalidCustomerTypeError())
          .RuleFor(x => x.Name).ApplyTemplate(ValidatorTemplate.NameRules)
          .RuleFor(x => x.Code).ApplyTemplate(CodeRules)
          .RuleForValue(codeExists).IsFalse(new DuplicateCustomerCodeError(request.Code));

      var isValid = validator.Validate(request);

      return isValid ? Result.Success() : Result.Failure(validator.Messages);
   }

    public Result ValidateUpdate(Guid id, CustomerUpdateRequest request)
    {
        var validator = new FluentValidator<CustomerUpdateRequest>()
            .RuleFor(x => x.Name).ApplyTemplate(ValidatorTemplate.NameRules)
            .RuleFor(x => x.Code).IsRequired().MinLength(3);

        var isValid = validator.Validate(request);

        return isValid ? Result.Success() : Result.Failure(validator.Messages);
    }
}
