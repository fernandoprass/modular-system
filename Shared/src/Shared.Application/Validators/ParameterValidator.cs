using Myce.FluentValidator;
using Myce.Response;
using Shared.Application.Contracts;
using Shared.Domain;
using Shared.Domain.DTOs.Requests;
using Shared.Domain.Entities;
using Shared.Domain.Enums;
using Shared.Domain.Messages;
using System.Globalization;

namespace Shared.Application.Validators;

public class ParameterValidator : IParameterValidator
{
   private static void MemberKey<T>(RuleBuilder<T, string> rb) where T : class
                     => rb.IsRequired().MinLength(2).IsAlphaNumeric();

   public Result ValidateCreate(ParameterCreateRequest request, bool keyExists)
   {
      var validator = new FluentValidator<ParameterCreateRequest>()
         .RuleFor(x => x.Module).ApplyTemplate(MemberKey)
         .RuleFor(x => x.Group).ApplyTemplate(MemberKey)
         .RuleFor(x => x.Name).ApplyTemplate(MemberKey)
         .RuleFor(x => x.Description).IsRequired()
         .RuleFor(x => x.Type).IsRequired()
         .RuleFor(x => x.Value).IsRequired()
         .RuleForValue(keyExists).IsFalse(new DuplicateParameterError(request.Module, request.Group, request.Name));

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
