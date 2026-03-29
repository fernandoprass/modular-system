namespace IAM.Domain.Interfaces;

//todo move it to shared project
public interface IUserContext
{
   Guid CustomerId { get; }
   bool IsAuthenticated { get; } 
   bool IsSystemAdmin { get; }
   Guid UserId { get; }
}