using System.Security.Claims;
using IAM.Domain;

namespace IAM.Application.Extensions;

public static class ClaimsPrincipalExtensions
{
   public static Guid GetCustomerId(this ClaimsPrincipal user)
   {
      var value = user.FindFirst(Const.Security.Claim.CustomerId)?.Value;
      return Guid.TryParse(value, out var id) ? id : Guid.Empty;
   }

   public static Guid GetUserId(this ClaimsPrincipal user)
   {
      var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user.FindFirst("sub")?.Value;

      return Guid.TryParse(value, out var id) ? id : Guid.Empty;
   }

   public static bool IsInRole(this ClaimsPrincipal user, string role)
   {
      return user.HasClaim(ClaimTypes.Role, role);
   }
}