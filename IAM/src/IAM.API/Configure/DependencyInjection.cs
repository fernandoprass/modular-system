using IAM.Core.Services;
using IAM.Core.Validators;
using IAM.Domain.QueryRepositories;
using IAM.Domain.Repositories;
using IAM.Infrastructure.QueryRepositories;
using IAM.Infrastructure.Repositories;

namespace IAM.API.Configure
{
   public static class DependencyInjection
   {
      public static void RegisterRepositories(WebApplicationBuilder builder)
      {
         // Register Unit of Work and Repositories
         builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
         builder.Services.AddScoped<IUserRepository, UserRepository>();
         builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
         builder.Services.AddScoped<IUserQueryRepository, UserQueryRepository>();
         builder.Services.AddScoped<ICustomerQueryRepository, CustomerQueryRepository>();
      }

      public static void RegisterServices(WebApplicationBuilder builder)
      {
         // Register services
         builder.Services.AddScoped<ICustomerService, CustomerService>();
         builder.Services.AddScoped<IUserService, UserService>();
         builder.Services.AddScoped<IAuthService, AuthService>();
         builder.Services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
      }
   }
}
