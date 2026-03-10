using IAM.Application.Contracts;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.Messages.Errors;
using Myce.FluentValidator;
using Myce.Response;

namespace IAM.Application.Validators;

public class CustomerValidator : ICustomerValidator
{
   public Result ValidateCreate(CustomerCreateRequest request)
   {
      var isPersonWithInvalidCode = request.Type == CustomerType.Person && 
                                    (string.IsNullOrEmpty(request.Code) || request.Code.Length != 10 || !request.Code.All(char.IsLetterOrDigit));

      var validator = new FluentValidator<CustomerCreateRequest>()
          .RuleFor(x => x.Name).ApplyTemplate(TemplateRulesValidator.NameRules)
          .RuleFor(x => x.Code).IsRequired().MinLength(3)
          .RuleFor(x => x.User.Name).ApplyTemplate(TemplateRulesValidator.NameRules)
          .RuleFor(x => x.User.Email).IsRequired().IsValidEmailAddress()
          .RuleFor(x => x.User.Password).ApplyTemplate(TemplateRulesValidator.PasswordRules)
          .RuleForValue(isPersonWithInvalidCode).IsFalse(new InvalidCodeFormatError());

      var isValid = validator.Validate(request);

      return isValid ? Result.Success() : Result.Failure(validator.Messages);
   }

    public Result ValidateUpdate(Guid id, CustomerUpdateRequest request)
    {
        var validator = new FluentValidator<CustomerUpdateRequest>()
            .RuleFor(x => x.Name).ApplyTemplate(TemplateRulesValidator.NameRules)
            .RuleFor(x => x.Code).IsRequired().MinLength(3);

        var isValid = validator.Validate(request);

        return isValid ? Result.Success() : Result.Failure(validator.Messages);
    }
}
