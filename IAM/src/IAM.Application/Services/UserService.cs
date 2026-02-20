using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;
using IAM.Domain.Entities;
using IAM.Domain.QueryRepositories;
using IAM.Domain.Repositories;
using Isopoh.Cryptography.Argon2;
using IAM.Application.Validators.User;
using IAM.Domain.Messages;
using IAM.Domain.Messages.Errors;
using Myce.Response;

namespace IAM.Application.Services;


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
      return await _userQueryRepository.GetByIdAsync(id);
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

   public async Task<Result<User>> CreateUserAsync(UserCreateRequest request)
   {
      //todo fix it to use dependency injection and not create new instance every time
      var validate = new UserCreateValidator(_userQueryRepository).Validate(request);

      if (!validate.IsValid)
      {
         return Result<User>.Failure(validate.Messages);
      }

      var existingUser = await _userQueryRepository.GetByEmailAsync(request.Email);
      if (existingUser != null)
      {
         return Result<User>.Failure(new UserEmailAlreadyExistError(request.Email));
      }

      var user = new User
      {
         Id = Guid.CreateVersion7(),
         Name = request.Name,
         Email = request.Email,
         PasswordHash = Argon2.Hash(request.Password),
         CreatedAt = DateTime.UtcNow,
         CustomerId = request.CustomerId
      };

      var result = await CreateAsync(user);

      return result is not null ? Result<User>.Success(result) : Result<User>.Failure(new FailedToRecordDataError());
   }

   public async Task<UserDto?> ValidateCredentialsAsync(string email, string password)
   {
      //_userValidator.ValidateCredentials(email, password);

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
      };
   }

   public async Task UpdateAsync(User user)
   {
      user.UpdatedAt = DateTime.UtcNow;
      _unitOfWork.Users.Update(user);
      await _unitOfWork.SaveChangesAsync();
   }

   public async Task UpdatePasswordAsync(UserUpdatePasswordRequest request)
   {
      //_userValidator.Validate(request);

      var user = await _userQueryRepository.GetByEmailWithPasswordAsync(request.Email);
      if (user == null)
      {
         throw new InvalidOperationException("User not found");
      }

      var isValid = Argon2.Verify(request.PasswordOld, user.PasswordHash);
      if (!isValid)
      {
         throw new InvalidOperationException("Invalid old password");
      }

      // We need to retrieve the Domain User entity to update it, because QueryRepository returns DTO/projection
      // Assuming IUserRepository has GetByIdAsync that returns User entity.
      var userEntity = await _userRepository.GetByIdAsync(user.Id);
      if (userEntity == null)
      {
          throw new InvalidOperationException("User entity not found");
      }

      userEntity.PasswordHash = Argon2.Hash(request.PasswordNew);
      userEntity.UpdatedAt = DateTime.UtcNow;

      _unitOfWork.Users.Update(userEntity);
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