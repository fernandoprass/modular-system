namespace Shared.Domain.Interfaces;

public interface ISharedUnitOfWork : IUnitOfWork
{
   IParameterRepository Parameters { get; }
   IParameterOverrideRepository ParameterOverrides { get; }
}