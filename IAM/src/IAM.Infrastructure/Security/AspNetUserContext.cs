using IAM.Application.Extensions;
using IAM.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace IAM.Infrastructure.Security;

public class AspNetUserContext : IUserContext
{
   private readonly IHttpContextAccessor _accessor;

   public AspNetUserContext(IHttpContextAccessor accessor)
   {
      _accessor = accessor;
   }

   public Guid CustomerId => _accessor.HttpContext?.User.GetCustomerId() ?? Guid.Empty;

   public bool IsAuthenticated => _accessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

   public Guid UserId
   {
      get
      {
         var claim = _accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
         return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
      }
   }
   
}