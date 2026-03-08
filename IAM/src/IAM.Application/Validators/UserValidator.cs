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

      public Result ValidateCreate(UserCreateRequest request, bool emailAlreadyExists, bool customerExists)
      {
         var validator = new FluentValidator<UserCreateRequest>()
            .RuleFor(x => x.Name).ApplyTemplate(NameRules)
            .RuleFor(x => x.Email)
                  .IsRequired()
                  .IsValidEmailAddress()
                  .Custom(_ => !emailAlreadyExists, new EmailAlreadyExistError(request.Email))
            .RuleFor(x => x.Password).ApplyTemplate(PasswordRules)
            .RuleFor(x => x.CustomerId)
                  .IsRequired()
                  .Custom(_ => customerExists, new NotFoundError(Const.Entity.Customer));

         var isValid = validator.Validate(request);

         return isValid ? Result.Success() : Result.Failure(validator.Messages);
      }

      public Result ValidateUpdate(Guid? id, UserUpdateRequest request)
      {
         var validator = new FluentValidator<UserUpdateRequest>()
            .RuleFor(x => x.Name).ApplyTemplate(NameRules)
            .Custom(id is not null, new NotFoundError(Const.Entity.User));

         var isValid = validator.Validate(request);

         return isValid ? Result.Success() : Result.Failure(validator.Messages);
      }

      public Result ValidateUpdatePassword(User? user, UserUpdatePasswordRequest request)
      {
         var isOldPasswordCorrect = user != null &&
                                    Argon2.Verify(request.PasswordOld, user.PasswordHash);

         var validator = new FluentValidator<UserUpdatePasswordRequest>()
            .RuleForValue(user).IsNotNull(new NotFoundError(Const.Entity.User))
            .RuleFor(x => x.Email).IsRequired().IsValidEmailAddress()
            .RuleFor(x => x.PasswordOld).IsRequired()
            .RuleForValue(isOldPasswordCorrect).IsTrue(new PasswordNotValidError())
            .RuleFor(x => x.PasswordNew).ApplyTemplate(PasswordRules);

         var isValid = validator.Validate(request);

         return isValid ? Result.Success() : Result.Failure(validator.Messages);
      }


      #region Templates
      private static void NameRules<T>(RuleBuilder<T, string> rb) where T : class
      {
         rb.IsRequired().MinLength(3);
      }

      /// <summary>
      /// Reusable template for strong password validation without Regex.
      /// </summary>
      private static void PasswordRules<T>(RuleBuilder<T, string> rb) where T : class
      {
         // 1. Has minimum 8 characters in length
         rb.MinLength(8, new PasswordMinLengthError());

         // 2. At least one uppercase English letter
         rb.Custom(pwd => !string.IsNullOrEmpty(pwd) && pwd.Any(char.IsUpper), new PasswordMissingUppercaseError());

         // 3. At least one lowercase English letter
         rb.Custom(pwd => !string.IsNullOrEmpty(pwd) && pwd.Any(char.IsLower), new PasswordMissingLowercaseError());

         // 4. At least one digit
         rb.Custom(pwd => !string.IsNullOrEmpty(pwd) && pwd.Any(char.IsDigit), new PasswordMissingDigitError());

         // 5. At least one special character
         // We define special characters manually to match your previous requirement: #?!@$%^&*-
         char[] specialChars = { '#', '?', '!', '@', '$', '%', '^', '&', '*', '-', '_', '.' };
         rb.Custom(pwd => !string.IsNullOrEmpty(pwd) && pwd.Any(c => specialChars.Contains(c)), new PasswordMissingSpecialError());
      }
      #endregion
   }
}
