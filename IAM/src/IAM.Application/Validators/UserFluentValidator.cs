using IAM.Application.Contracts;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.Messages;
using IAM.Domain.QueryRepositories;
using Myce.FluentValidator;
using Myce.Response;

namespace IAM.Application.Validators
{
   public class UserFluentValidator : IUserFluentValidator
   {
      private readonly IUserQueryRepository _userQueryRepository;

      public UserFluentValidator(IUserQueryRepository userQueryRepository)
      {
         _userQueryRepository = userQueryRepository;
      }

      public Result ValidateCreate(UserCreateRequest request)
      {
         Func<string, bool> isNewEmail = VerifyIfEmailExists(request.Email);

         var validator = new FluentValidator<UserCreateRequest>()
             .RuleFor(x => x.Name).IsRequired().MinLength(3)
             .RuleFor(x => x.Email)
               .IsRequired()
               .IsValidEmailAddress()
               .Custom(isNewEmail, new EmailAlreadyExistError(request.Email))
             .RuleFor(x => x.Password).ApplyTemplate(PasswordRules)
             .RuleFor(x => x.CustomerId).IsRequired();

         var isValid = validator.Validate(request);

         return isValid ? Result.Success() : Result.Failure(validator.Messages);
      }

      private Func<string, bool> VerifyIfEmailExists(string email)
      {
         var UserId = _userQueryRepository.GetIdByEmailAsync(email);

         Func<string, bool> isNewEmail = email => !UserId.HasValue || UserId.Value == Guid.Empty;
         return isNewEmail;
      }

      #region Templates
      /// <summary>
      /// Reusable template for strong password validation without Regex.
      /// </summary>
      private static void PasswordRules<T>(RuleBuilder<T, string> rb) where T : class
      {
         // 1. Has minimum 8 characters in length
         rb.MinLength(8, new UserPasswordMinLengthError());

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
