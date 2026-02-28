using IAM.Application.Contracts;
using IAM.Domain;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;
using IAM.Domain.Entities;
using IAM.Domain.Mappers;
using IAM.Domain.Messages;
using IAM.Domain.Messages.Errors;
using IAM.Domain.QueryRepositories;
using IAM.Domain.Repositories;
using Isopoh.Cryptography.Argon2;
using Myce.Response;
using System.Reflection.Metadata;

namespace IAM.Application.Services;


public class UserService : IUserService
{
   private readonly IUnitOfWork _unitOfWork;
   private readonly IUserFluentValidator _userFluentValidator;
   private readonly IUserRepository _userRepository;
   private readonly IUserQueryRepository _userQueryRepository;

   public UserService(
       IUnitOfWork unitOfWork,
       IUserFluentValidator userFluentValidator,
       IUserRepository userRepository,
       IUserQueryRepository userQueryRepository)
   {
      _unitOfWork = unitOfWork;
      _userFluentValidator = userFluentValidator;
      _userRepository = userRepository;
      _userQueryRepository = userQueryRepository;
   }

   public async Task<UserDto?> GetByIdAsync(Guid id)
   {
      return await _userQueryRepository.GetByIdAsync(id);
   }

   public async Task<UserDto?> GetByEmailAsync(string email)
   {
      return await _userQueryRepository.GetByEmailAsync(email);
   }

   public async Task<IEnumerable<UserLiteDto>> GetByCustomerIdAsync(Guid customerId)
   {
      return await _userQueryRepository.GetByCustomerIdAsync(customerId);
   }

   public async Task<Result<UserDto>> CreateUserAsync(UserCreateRequest request)
   {
      var validator = _userFluentValidator.ValidateCreate(request);
      if (validator.HasError)
      {
         return Result<UserDto>.Failure(validator.Messages);
      }

      var user = new User
      {
         Id = Guid.CreateVersion7(),
         Name = request.Name,
         Email = request.Email,
         PasswordHash = Argon2.Hash(request.Password),
         IsActive = true,
         CreatedAt = DateTime.UtcNow,
         CustomerId = request.CustomerId
      };

      var result = await CreateAsync(user);

      return result is not null
             ? Result<UserDto>.Success(result.ToUserDto()) 
             : Result<UserDto>.Failure(new FailedToRecordDataError());
   }

   private async Task<User> CreateAsync(User user)
   {
      await _unitOfWork.Users.AddAsync(user);
      await _unitOfWork.SaveChangesAsync();
      return user;
   }

   public async Task<Result> UpdateAsync(Guid id, UserUpdateRequest request)
   {
      var user = await _userRepository.GetByIdAsync(id);

      var validator = _userFluentValidator.ValidateUpdate(user?.Id, request);
      if (validator.HasError)
      {
         return Result<UserDto>.Failure(validator.Messages);
      }

      user.Name = request.Name;
      user.IsActive = request.IsActive;
      user.UpdatedAt = DateTime.UtcNow;

      _unitOfWork.Users.Update(user);
      await _unitOfWork.SaveChangesAsync();
      return Result.Success();
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

   public async Task UpdateLastLoginAsync(Guid id)
   {
      var userEntity = await _userRepository.GetByIdAsync(id);
      userEntity.LastLoginAt = DateTime.UtcNow;
      await _unitOfWork.SaveChangesAsync();
   }
}