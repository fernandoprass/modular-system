using IAM.Domain.DTOs.Requests;
using Myce.FluentValidator;

namespace IAM.Application.Validators;

public class UserValidator
{
   public void Validate(UserUpdatePasswordRequest request)
   {
      var validator = new FluentValidator<UserUpdatePasswordRequest>()
          .RuleFor(x => x.Email).IsRequired().IsValidEmailAddress()
          .RuleFor(x => x.PasswordOld).IsRequired()
          .RuleFor(x => x.PasswordNew).IsRequired().MinLength(8);

      if (!validator.Validate(request))
      {
         throw new ArgumentException("Validation failed for UpdatePasswordRequest");
      }
   }

   public void ValidateCredentials(string email, string password)
   {
      // Using a record to validate loose parameters
      var credentials = new Credentials { Email = email, Password = password };
      var validator = new FluentValidator<Credentials>()
          .RuleFor(x => x.Email).IsRequired().IsValidEmailAddress()
          .RuleFor(x => x.Password).IsRequired();

      if (!validator.Validate(credentials))
      {
         // var errors = string.Join("; ", result.Messages.Select(m => m.Text)); // Cannot access messages yet
         throw new ArgumentException("Validation failed for credentials");
      }
   }

   private record Credentials
   {
      public string Email { get; init; } = string.Empty;
      public string Password { get; init; } = string.Empty;
   }
}
