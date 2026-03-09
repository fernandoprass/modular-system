using IAM.Application.Contracts;
using IAM.Application.Orchestrators;
using IAM.Application.Services;
using IAM.Application.Validators;
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
         builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
         builder.Services.AddScoped<IUserRepository, UserRepository>();
         builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
         builder.Services.AddScoped<IUserQueryRepository, UserQueryRepository>();
         builder.Services.AddScoped<ICustomerQueryRepository, CustomerQueryRepository>();
      }

      public static void RegisterOrchestrators(WebApplicationBuilder builder)
      {
         builder.Services.AddScoped<IUserOrchestrator, UserOrchestrator>();
      }

      public static void RegisterServices(WebApplicationBuilder builder)
      {
         builder.Services.AddScoped<ICustomerService, CustomerService>();
         builder.Services.AddScoped<IUserService, UserService>();
         builder.Services.AddScoped<IAuthService, AuthService>();
         builder.Services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
      }

      public static void RegisterValidators(WebApplicationBuilder builder)
      {
         builder.Services.AddScoped<IUserValidator, UserValidator>();
         builder.Services.AddScoped<ICustomerValidator, CustomerValidator>();
      }
   }
}
