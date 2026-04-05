using IAM.Application.Contracts;
using IAM.Application.Orchestrators;
using IAM.Application.Services;
using IAM.Application.Validators;
using IAM.Domain.QueryRepositories;
using IAM.Domain.Repositories;
using IAM.Infrastructure;
using IAM.Infrastructure.QueryRepositories;
using IAM.Infrastructure.Repositories;
using Shared.Application.Contracts;
using Shared.Application.Services;
using Shared.Application.Validators;
using Shared.Domain.Interfaces;
using Shared.Infrastructure.UoW;

namespace IAM.API.Configure
{
   public static class DependencyInjection
   {
      public static void RegisterRepositories(WebApplicationBuilder builder)
      {
         builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
         builder.Services.AddScoped<ICustomerQueryRepository, CustomerQueryRepository>();
         builder.Services.AddScoped<IRoleRepository, RoleRepository>();
         builder.Services.AddScoped<IRoleQueryRepository, RoleQueryRepository>();
         builder.Services.AddScoped<IUserRepository, UserRepository>();
         builder.Services.AddScoped<IUserQueryRepository, UserQueryRepository>();

         builder.Services.AddScoped<IUnitOfWork<IamDbContext>, UnitOfWork<IamDbContext>>();
    //     builder.Services.AddScoped<IUnitOfWork<SharedDbContext>, UnitOfWork<SharedDbContext>>();
      }

      public static void RegisterOrchestrators(WebApplicationBuilder builder)
      {
         builder.Services.AddScoped<IRegisterOrchestrator, ResgisterOrchestrator>();
      }

      public static void RegisterServices(WebApplicationBuilder builder)
      {
         builder.Services.AddScoped<IAuthService, AuthService>(); 
         builder.Services.AddScoped<ICustomerService, CustomerService>();
         builder.Services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
         builder.Services.AddScoped<IRoleService, RoleService>();
         builder.Services.AddScoped<IUserService, UserService>();
         builder.Services.AddScoped<IParameterService, ParameterService>();
      }

      public static void RegisterValidators(WebApplicationBuilder builder)
      {
         builder.Services.AddScoped<ICustomerValidator, CustomerValidator>();
         builder.Services.AddScoped<IUserValidator, UserValidator>();
         builder.Services.AddScoped<IRoleValidator, RoleValidator>();
         builder.Services.AddScoped<IParameterValidator, ParameterValidator>();
      }
   }
}
