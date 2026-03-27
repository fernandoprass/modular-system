namespace IAM.Domain.Interfaces;

//todo move it to shared project
public interface IUserContext
{
   Guid CustomerId { get; }
   bool IsAuthenticated { get; } 
   bool IsSuperUser { get; }
   Guid UserId { get; }
}