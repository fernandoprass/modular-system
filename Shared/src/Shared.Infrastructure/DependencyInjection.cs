using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Domain;
using Shared.Domain.Interfaces;
using Shared.Domain.Repositories;
using Shared.Infrastructure.Repositories;
using Shared.Infrastructure.UoW;

namespace Shared.Infrastructure;

public static class DependencyInjection
{
   public static IServiceCollection AddSharedInfrastructure(
       this IServiceCollection services,
       IConfiguration configuration)
   {
      var connectionString = configuration.GetConnectionString(Const.System.DbConnectionName);
      services.AddDbContext<SharedDbContext>(options => options.UseNpgsql(connectionString));

      services.AddScoped<ISharedUnitOfWork, SharedUnitOfWork>();

      services.AddScoped(typeof(IBaseRepository<,>), typeof(BaseRepository<,>));
      services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

      services.AddScoped<IParameterRepository, ParameterRepository>();
      services.AddScoped<IParameterOverrideRepository, ParameterOverrideRepository>();

      return services;
   }
}