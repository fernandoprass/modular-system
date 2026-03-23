namespace IAM.Domain.DTOs.Requests
{
   public record RoleAssignRequest(Guid UserId, IEnumerable<Guid> RoleIds);
}
