namespace Shared.Domain.Interfaces
{
   public interface IUserContext
   {
      Guid CustomerId { get; }
      bool IsAuthenticated { get; }
      bool IsSystemAdmin { get; }
      Guid UserId { get; }
   }
}
