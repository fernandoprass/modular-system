using System.Globalization;
using IAM.Application.Contracts;
using IAM.Domain;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.Entities;
using IAM.Domain.Enums;
using IAM.Domain.Messages.Errors;
using Myce.FluentValidator;
using Myce.Response;

namespace IAM.Application.Validators
{
   public class ParameterValidator : IParameterValidator
   {
      public Result ValidateCreate(ParameterCreateRequest request, bool keyExists)
      {
         var validator = new FluentValidator<ParameterCreateRequest>()
            .RuleFor(x => x.Key)
               .IsRequired()
               .Matches(@"^([a-zA-Z0-9]{2,})\.([a-zA-Z0-9]{2,})\.([a-zA-Z0-9]{2,})$", new InvalidParameterKeyFormatError())
            .RuleFor(x => x.Description).IsRequired()
            .RuleFor(x => x.Type).IsRequired()
            .RuleFor(x => x.Value).IsRequired()
            .RuleForValue(keyExists).IsFalse(new DuplicateParameterError(request.Key));

         var isValid = validator.Validate(request);
         if (!isValid) return Result.Failure(validator.Messages);

         return ValidateValueFormat(request.Value, request.Type);
      }

      public Result ValidateUpdate(Parameter? parameter, ParameterUpdateRequest request)
      {
         var validator = new FluentValidator<ParameterUpdateRequest>()
            .RuleForValue(parameter).IsNotNull(new NotFoundError(Const.Entity.Parameter))
            .RuleFor(x => x.Name).IsRequired()
            .RuleFor(x => x.Description)
            .RuleFor(x => x.Value).IsRequired();

         var isValid = validator.Validate(request);
         if (!isValid) return Result.Failure(validator.Messages);

         return ValidateValueFormat(request.Value, parameter.Type);
      }

      public Result ValidateCustomerUpdate(Parameter? parameter, ParameterCustomerUpdateRequest request)
      {
         var validator = new FluentValidator<ParameterCustomerUpdateRequest>()
            .RuleForValue(parameter).IsNotNull(new NotFoundError(Const.Entity.Parameter))
            .RuleForValue(parameter?.IsCustomerEditable ?? false).IsTrue(new ParameterNotCustomerEditableError())
            .RuleFor(x => x.Value).IsRequired();

         var isValid = validator.Validate(request);
         if (!isValid) return Result.Failure(validator.Messages);

         return ValidateValueFormat(request.Value, parameter.Type);
      }

      private static Result ValidateValueFormat(string value, ParameterType type)
      {
         bool isValid = type switch
         {
            ParameterType.Boolean => bool.TryParse(value, out _),
            ParameterType.Integer => int.TryParse(value, out _),
            ParameterType.Numeric => decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _),
            ParameterType.DateTime => DateTime.TryParse(value, out _),
            ParameterType.Date => DateTime.TryParse(value, out _),
            ParameterType.Time => TimeSpan.TryParse(value, out _),
            _ => true
         };

         if (!isValid)
         {
            return Result.Failure(new InvalidParameterValueFormatError(type.ToString()));
         }

         return Result.Success();
      }
   }
}
