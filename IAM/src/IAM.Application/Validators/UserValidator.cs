using IAM.Application.Contracts;
using IAM.Domain;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.Entities;
using IAM.Domain.Messages;
using IAM.Domain.Messages.Errors;
using Isopoh.Cryptography.Argon2;
using Myce.FluentValidator;
using Myce.Response;

namespace IAM.Application.Validators
{
   public class UserValidator : IUserValidator
   {
      public UserValidator() { }

      public Result ValidateCreate(UserCreateRequest request, bool customerExists, bool emailAlreadyExists)
      {
         var validator = new FluentValidator<UserCreateRequest>()
               .RuleFor(x => x.Name).ApplyTemplate(ValidatorTemplate.NameRules)
               .RuleFor(x => x.Email).ApplyTemplate(ValidatorTemplate.EmailRules)
               .RuleFor(x => x.Password).ApplyTemplate(ValidatorTemplate.PasswordRules)
               .RuleFor(x => x.CustomerId).IsRequired()
               .RuleForValue(emailAlreadyExists).IsFalse(new EmailAlreadyExistError(request.Email))
               .RuleForValue(customerExists).IsFalse(new NotFoundError(Const.Entity.Customer));

         var isValid = validator.Validate(request);

         return isValid ? Result.Success() : Result.Failure(validator.Messages);
      }

      public Result ValidateCreateForNewCustomer(CustomerUserCreateRequest request, bool emailAlreadyExists)
      {
         var validator = new FluentValidator<CustomerUserCreateRequest>()
               .RuleFor(x => x.Name).ApplyTemplate(ValidatorTemplate.NameRules)
               .RuleFor(x => x.Email).ApplyTemplate(ValidatorTemplate.EmailRules)
               .RuleFor(x => x.Password).ApplyTemplate(ValidatorTemplate.PasswordRules)
               .RuleForValue(emailAlreadyExists).IsFalse(new EmailAlreadyExistError(request.Email));

         var isValid = validator.Validate(request);

         return isValid ? Result.Success() : Result.Failure(validator.Messages);
      }

      public Result ValidateUpdate(Guid? id, UserUpdateRequest request)
      {
         var validator = new FluentValidator<UserUpdateRequest>()
            .RuleFor(x => x.Name).ApplyTemplate(ValidatorTemplate.NameRules)
            .Custom(id is not null, new NotFoundError(Const.Entity.User));

         var isValid = validator.Validate(request);

         return isValid ? Result.Success() : Result.Failure(validator.Messages);
      }

      public Result ValidateUpdatePassword(User? user, UserUpdatePasswordRequest request)
      {
         var isOldPasswordCorrect = user != null &&
                                    Argon2.Verify(user.PasswordHash, request.PasswordOld);

         var validator = new FluentValidator<UserUpdatePasswordRequest>()
            .RuleForValue(user).IsNotNull(new NotFoundError(Const.Entity.User))
            .RuleFor(x => x.Email).IsRequired().IsValidEmailAddress()
            .RuleFor(x => x.PasswordOld).IsRequired()
            .RuleForValue(isOldPasswordCorrect).IsTrue(new PasswordNotValidError())
            .RuleFor(x => x.PasswordNew).ApplyTemplate(ValidatorTemplate.PasswordRules);

         var isValid = validator.Validate(request);

         return isValid ? Result.Success() : Result.Failure(validator.Messages);
      }


   }
}
