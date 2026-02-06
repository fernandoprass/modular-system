using IAM.Domain.DTOs;
using IAM.Domain.DTOs.Responses;
using IAM.Domain.QueryRepositories;
using Microsoft.EntityFrameworkCore;

namespace IAM.Infrastructure.QueryRepositories;

public class UserQueryRepository : IUserQueryRepository
{
    private readonly IamDbContext _context;

    public UserQueryRepository(IamDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .AsNoTracking()
            .Include(u => u.Customer)
            .Where(u => u.Email == email)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                CustomerId = u.CustomerId,
                CustomerName = u.Customer.Name,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }
    
    public async Task<UserPasswordDto?> GetByEmailWithPasswordAsync(string email)
    {
        return await _context.Users
            .AsNoTracking()
            .Include(u => u.Customer)
            .Where(u => u.Email == email)
            .Select(u => new UserPasswordDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                CustomerId = u.CustomerId,
                CustomerName = u.Customer.Name,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                PasswordHash = u.PasswordHash
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<UserDto>> GetByCustomerIdAsync(Guid customerId)
    {
        return await _context.Users
            .AsNoTracking()
            .Include(u => u.Customer)
            .Where(u => u.CustomerId == customerId)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                CustomerId = u.CustomerId,
                CustomerName = u.Customer.Name,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .Include(u => u.Customer)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                CustomerId = u.CustomerId,
                CustomerName = u.Customer.Name,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            })
            .ToListAsync();
    }
}