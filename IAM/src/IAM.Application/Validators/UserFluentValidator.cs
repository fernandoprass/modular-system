using IAM.Application.Contracts;
using IAM.Domain;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.Messages;
using IAM.Domain.Messages.Errors;
using IAM.Domain.QueryRepositories;
using IAM.Domain.Repositories;
using Microsoft.IdentityModel.Tokens;
using Myce.FluentValidator;
using Myce.Response;

namespace IAM.Application.Validators
{
   public class UserFluentValidator : IUserFluentValidator
   {
      private readonly IUserRepository _userRepository;
      private readonly ICustomerQueryRepository _customerQueryRepository;
      private readonly IUserQueryRepository _userQueryRepository;


      public UserFluentValidator(
         ICustomerQueryRepository customerQueryRepository,
         IUserQueryRepository userQueryRepository, 
         IUserRepository userRepository)
      {
         _customerQueryRepository = customerQueryRepository;
         _userQueryRepository = userQueryRepository;
         _userRepository = userRepository;
      }

      public async Task<Result> ValidateCreateAsync(UserCreateRequest request)
      {
         var isNewEmail = await VerifyIfEmailExistsAsync(request.Email);

         var customerDto = await _customerQueryRepository.GetByIdAsync(request.CustomerId);

         var customerExists = await VerifyIfCustomerExistsAsync(request.CustomerId);

         var validator = new FluentValidator<UserCreateRequest>()
             .RuleForValue(customerDto).IsNotNull(new NotFoundError(Const.Entity.Customer))
             .RuleFor(x => x.Name).ApplyTemplate(NameRules)
             .RuleFor(x => x.Email)
               .IsRequired()
               .IsValidEmailAddress()
               .Custom(isNewEmail, new EmailAlreadyExistError(request.Email))
             .RuleFor(x => x.Password).ApplyTemplate(PasswordRules)
             .RuleFor(x => x.CustomerId).IsRequired();

         var isValid = validator.Validate(request);

         return isValid ? Result.Success() : Result.Failure(validator.Messages);
      }

      public async Task<Result> ValidateUpdateAsync(Guid? id, UserUpdateRequest request)
      {
         //todo should validade id here
         var validator = new FluentValidator<UserUpdateRequest>()
            .RuleFor(x => x.Name).ApplyTemplate(NameRules);

         var isValid = validator.Validate(request);

         return isValid ? Result.Success() : Result.Failure(validator.Messages);
      }

      private async Task<Func<string, bool>> VerifyIfCustomerExistsAsync(Guid id)
      {
         var customer = await _customerQueryRepository.GetByIdAsync(id);

         Func<string, bool> customerExists = c => customer is not null;

         return customerExists;
      }

      private async Task<Func<string, bool>> VerifyIfEmailExistsAsync(string email)
      {
         var userId = await _userQueryRepository.GetIdByEmailAsync(email);

         Func<string, bool> isNewEmail = u => userId == Guid.Empty;
         
         return isNewEmail;
      }

      private Func<string, bool> VerifyUserExists(Guid? id)
      {
         Func<string, bool> userExists = u => id.HasValue && id.Value != Guid.Empty;
         return userExists;
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
