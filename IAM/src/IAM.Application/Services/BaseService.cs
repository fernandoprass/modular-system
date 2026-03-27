using IAM.Domain.Interfaces;
using IAM.Domain.Messages;
using Myce.Response;

namespace IAM.Application.Services
{
   public class BaseService
   {
      protected readonly IUserContext _userContext;

      protected BaseService(IUserContext userContext)
      {
         _userContext = userContext;
      }

      protected async Task<Result> ExecuteIfUserOwnsAsync(Guid? resourceCustomerId, Func<Task<Result>> actionAsync)
      {
         if (!IsUserAlllowedToAccess(resourceCustomerId))
         {
            return Result.Failure(new ForbiddenCustomerError());
         }

         return await actionAsync();
      }

      protected async Task<TResult> ExecuteIfUserOwnsAsync<TResult>(Guid? resourceCustomerId, Func<Task<TResult>> actionAsync) where TResult : Result
      {
         if (!IsUserAlllowedToAccess(resourceCustomerId))
         {
            var result = (TResult)Activator.CreateInstance(typeof(TResult))!;

            result.AddMessage(new ForbiddenCustomerError());

            return result;
         }

         return await actionAsync();
      }

      private bool IsUserAlllowedToAccess(Guid? resourceCustomerId)
      {
         return _userContext.IsSuperUser ||
                (resourceCustomerId.HasValue && resourceCustomerId == _userContext.CustomerId);
      }
   }
}
