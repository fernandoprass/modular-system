namespace Shared.Domain.Interfaces
{
   public interface IUserContext
   {
      Guid OwnerId { get; }
      bool IsAuthenticated { get; }
      bool IsSystemAdmin { get; }
      Guid UserId { get; }
   }
}
