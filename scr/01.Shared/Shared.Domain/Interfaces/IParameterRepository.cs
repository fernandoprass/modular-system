using Shared.Domain.Entities;
using System.Security.Cryptography;

namespace Shared.Domain.Interfaces;

internal interface IParameterRepository
{
   Task<Parameter?> GetByIdAsync(Guid id);
   Task AddAsync(Parameter parameter);
   void Update(Parameter parameter);
   Task DeleteAsync(Guid id);
   Task<bool> ExistsAsync(Guid id);
   Task<bool> ExistsAsync(string key);

   Task<Parameter?> GetByKeyAsync(string key);
 }
