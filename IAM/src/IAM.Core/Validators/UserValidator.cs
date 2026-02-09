using IAM.Domain.DTOs.Requests;
using Myce.Response;
using Myce.Validation;

namespace IAM.Core.Validators;

public class UserValidator
{
   public void Validate(UpdatePasswordRequest request)
   {
      var validator = new EntityValidator<UpdatePasswordRequest>()
          .RuleFor(x => x.Email).IsRequired().IsValidEmailAddress().Apply()
          .RuleFor(x => x.PasswordOld).IsRequired().Apply()
          .RuleFor(x => x.PasswordNew).IsRequired().MinLength(8).Apply();

      if (!validator.Validate(request))
      {
         throw new ArgumentException("Validation failed for UpdatePasswordRequest");
      }
   }

   public void ValidateCredentials(string email, string password)
   {
      // Using a record to validate loose parameters
      var credentials = new Credentials { Email = email, Password = password };
      var validator = new EntityValidator<Credentials>()
          .RuleFor(x => x.Email).IsRequired().IsValidEmailAddress().Apply()
          .RuleFor(x => x.Password).IsRequired().Apply();

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
