using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;
using IAM.Domain.Entities;
using IAM.Domain.QueryRepositories;
using IAM.Domain.Repositories;
using Isopoh.Cryptography.Argon2;

namespace IAM.Core.Services;

public class UserService : IUserService
{
   private readonly IUnitOfWork _unitOfWork;
   private readonly IUserRepository _userRepository;
   private readonly IUserQueryRepository _userQueryRepository;

   public UserService(
       IUnitOfWork unitOfWork,
       IUserRepository userRepository,
       IUserQueryRepository userQueryRepository)
   {
      _unitOfWork = unitOfWork;
      _userRepository = userRepository;
      _userQueryRepository = userQueryRepository;
   }

   public async Task<UserDto?> GetByIdAsync(Guid id)
   {
      var users = await _userQueryRepository.GetAllAsync();
      return users.FirstOrDefault(u => u.Id == id);
   }

   public async Task<IEnumerable<UserDto>> GetAllAsync()
   {
      return await _userQueryRepository.GetAllAsync();
   }

   public async Task<UserDto?> GetByEmailAsync(string email)
   {
      return await _userQueryRepository.GetByEmailAsync(email);
   }

   public async Task<IEnumerable<UserDto>> GetByCustomerIdAsync(Guid customerId)
   {
      return await _userQueryRepository.GetByCustomerIdAsync(customerId);
   }

   public async Task<User> CreateAsync(User user)
   {
      user.Id = Guid.CreateVersion7();
      user.CreatedAt = DateTime.UtcNow;
      await _unitOfWork.Users.AddAsync(user);
      await _unitOfWork.SaveChangesAsync();
      return user;
   }

   public async Task<User> CreateUserAsync(CreateUserRequest request)
   {
      // Check if email already exists
      var existingUser = await _userQueryRepository.GetByEmailAsync(request.Email);
      if (existingUser != null)
      {
         throw new InvalidOperationException("User with this email already exists");
      }

      // Generate salt and hash password
      var passwordHash = Argon2.Hash(request.Password);

      var user = new User
      {
         Name = request.Name,
         Email = request.Email,
         PasswordHash = passwordHash,
         CustomerId = request.CustomerId
      };

      return await CreateAsync(user);
   }

   public async Task<UserDto?> ValidateCredentialsAsync(string email, string password)
   {
      var user = await _userQueryRepository.GetByEmailWithPasswordAsync(email) ;
      if (user == null)
      {
         return null;
      }

      // Verify password
      var isValid = Argon2.Verify(password, user.PasswordHash);
      if (!isValid)
      {
         return null;
      }

      return new UserDto
      {
         Id = user.Id,
         Name = user.Name,
         Email = user.Email,
         CustomerId = user.CustomerId,
         CustomerName = user.CustomerName,
         CreatedAt = user.CreatedAt,
         UpdatedAt = user.UpdatedAt
      };
   }

   public async Task UpdateAsync(User user)
   {
      user.UpdatedAt = DateTime.UtcNow;
      _unitOfWork.Users.Update(user);
      await _unitOfWork.SaveChangesAsync();
   }

   public async Task DeleteAsync(Guid id)
   {
      await _unitOfWork.Users.DeleteAsync(id);
      await _unitOfWork.SaveChangesAsync();
   }

   public async Task<bool> ExistsAsync(Guid id)
   {
      return await _unitOfWork.Users.ExistsAsync(id);
   }
}