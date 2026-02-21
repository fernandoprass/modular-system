using IAM.Application.Contracts;
using IAM.Domain.DTOs.Requests;
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
         var emailExists = _userQueryRepository.GetIdByEmailAsync(request.Email);

         var validator = new FluentValidator<UserCreateRequest>()
             .RuleFor(x => x.Name).IsRequired().MinLength(3)
             .RuleFor(x => x.Email).IsRequired().IsValidEmailAddress()
             .RuleFor(x => x.Password).IsRequired().MinLength(8)
             .RuleFor(x => x.CustomerId).IsRequired();

         var isValid = validator.Validate(request);

         return isValid ? Result.Success() : Result.Failure(validator.Messages);
      }
   }
}
