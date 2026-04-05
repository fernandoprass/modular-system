using IAM.Domain.DTOs;
using IAM.Domain.DTOs.Responses;
using IAM.Domain.Entities;
using IAM.Domain.Mappers;
using IAM.Domain.QueryRepositories;
using Microsoft.EntityFrameworkCore;

namespace IAM.Infrastructure.QueryRepositories;

public class UserQueryRepository(IamDbContext context) : IUserQueryRepository
{
   private readonly IamDbContext _context = context;

   public async Task<UserDto?> GetByIdAsync(Guid id)
   {
      return await _context.Users
          .AsNoTracking()
          .Include(u => u.Customer)
          .Where(u => u.Id == id)
          .Select(u => u.ToUserDto())
          .SingleOrDefaultAsync();
   }

   public async Task<User?> GetByEmailAsync(string email)
   {
      return await _context.Users
          .AsNoTracking()
          .Include(u => u.Customer)
          .Where(u => u.Email == email)
          .Select(u => u)
          .SingleOrDefaultAsync();
   }

   public Task<Guid> GetIdByEmailAsync(string email)
   {
      return _context.Users
          .AsNoTracking()
          .Where(u => u.Email == email)
          .Select(u => u.Id)
          .SingleOrDefaultAsync();
   }

   public async Task<UserPasswordDto?> GetByEmailWithPasswordAsync(string email)
   {
      return await _context.Users
          .AsNoTracking()
          .Include(u => u.Customer)
          .Where(u => u.Email == email)
          .Select(u => u.ToUserPasswordDto())
          .SingleOrDefaultAsync();
   }

   public async Task<IEnumerable<UserLiteDto>> GetByCustomerIdAsync(Guid customerId)
   {
      return await _context.Users
          .AsNoTracking()
          .Where(u => u.CustomerId == customerId)
          .Select(u => new UserLiteDto
          {
             Id = u.Id,
             Name = u.Name,
             Email = u.Email,
             IsActive = u.IsActive
          })
          .ToListAsync();
   }
}