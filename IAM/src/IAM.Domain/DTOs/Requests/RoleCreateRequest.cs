namespace IAM.Domain.DTOs.Requests
{
   public record RoleCreateRequest(string Name, Guid? CustomerId, bool IsDefault = false);
}
