using IAM.Infrastructure.Security;
using Shared.Domain.Interfaces;

namespace IAM.API.Configure
{
   public static class UserContext
   {
      public static void Configure(WebApplicationBuilder builder)
      {
         builder.Services.AddHttpContextAccessor();
         builder.Services.AddScoped<IUserContext, AspNetUserContext>();
      }
   }
}
