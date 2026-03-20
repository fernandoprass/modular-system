using IAM.Application.Contracts;
using IAM.Domain;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;
using IAM.Domain.Entities;
using IAM.Domain.Interfaces;
using IAM.Domain.Mappers;
using IAM.Domain.Messages;
using IAM.Domain.Messages.Errors;
using IAM.Domain.Messages.Info;
using IAM.Domain.QueryRepositories;
using IAM.Domain.Repositories;
using Isopoh.Cryptography.Argon2;
using Myce.Response;

namespace IAM.Application.Services;

public class UserService : IUserService
{
   private readonly IUnitOfWork _unitOfWork;
   private readonly IUserContext _userContext;
   private readonly IUserValidator _userValidator;
   private readonly IUserRepository _userRepository;
   private readonly IUserQueryRepository _userQueryRepository;

   public UserService(
       IUnitOfWork unitOfWork,
       IUserContext userContext,
       IUserValidator userValidator,
       IUserRepository userRepository,
       IUserQueryRepository userQueryRepository)
   {
      _unitOfWork = unitOfWork;
      _userContext = userContext;
      _userValidator = userValidator;
      _userRepository = userRepository;
      _userQueryRepository = userQueryRepository;
   }

   public async Task<UserDto?> GetByIdAsync(Guid id)
   {
      return await _userQueryRepository.GetByIdAsync(id);
   }

   public async Task<IEnumerable<UserLiteDto>> GetByCustomerIdAsync(Guid customerId)
   {
      return await _userQueryRepository.GetByCustomerIdAsync(customerId);
   }

   public async Task<Result<UserDto>> CreateUserAsync(UserCreateRequest request,
                                                      bool customerExists)
   {
      bool emailExists = await EmailExistsAsync(request.Email);

      if (request.CustomerId != _userContext.UserId)
      {
         return Result<UserDto>.Failure(new ForbiddenCustomerError());
      }

      var validation = _userValidator.ValidateCreate(request, customerExists, emailExists);
      if (validation.HasError)
      {
         return Result<UserDto>.Failure(validation.Messages);
      }

      var user = User.Create(
          request.Name,
          request.Email,
          Argon2.Hash(request.Password),
          request.CustomerId);

      await _unitOfWork.Users.AddAsync(user);
      await _unitOfWork.SaveChangesAsync();

      return Result<UserDto>.Success(user.ToUserDto());
   }

   public async Task<Result> UpdateAsync(Guid id, UserUpdateRequest request)
   {
      var user = await _userRepository.GetByIdAsync(id);

      var validator = _userValidator.ValidateUpdate(user?.Id, request);
      if (validator.HasError)
      {
         return Result.Failure(validator.Messages);
      }

      user.Update(request.Name, request.IsActive);

      _unitOfWork.Users.Update(user);
      await _unitOfWork.SaveChangesAsync();
      return Result.Success(new SuccessInfo());
   }

   public async Task<Result> UpdatePasswordAsync(UserUpdatePasswordRequest request)
   {
      var user = await _userQueryRepository.GetByEmailAsync(request.Email);

      var validator = _userValidator.ValidateUpdatePassword(user, request);
      if (validator.HasError)
      {
         return Result.Failure(validator.Messages);
      }
      
      user.UpdatePassword(Argon2.Hash(request.PasswordNew));

      _unitOfWork.Users.Update(user);
      await _unitOfWork.SaveChangesAsync();
      return Result.Success(new SuccessInfo());
   }

   public async Task<Result> DeleteAsync(Guid id)
   {
      var user = await _userRepository.GetByIdAsync(id);

      if (user == null)
      {
         return Result.Failure(new NotFoundError(Const.Entity.User));
      }

      await _unitOfWork.Users.DeleteAsync(id);
      await _unitOfWork.SaveChangesAsync();
      return Result.Success(new SuccessInfo());
   }

   public async Task UpdateLastLoginAsync(Guid id)
   {
      var user = await _userRepository.GetByIdAsync(id);

      user.UpdateLastLogin();
      await _unitOfWork.SaveChangesAsync();
   }

   public async Task<Result> ValidateUserForNewCustomerAsync(CustomerUserCreateRequest request)
   {
      bool emailExists = await EmailExistsAsync(request.Email);

      return _userValidator.ValidateCreateForNewCustomer(request, emailExists);
   }

   private async Task<bool> EmailExistsAsync(string email)
   {
      var userId = await _userQueryRepository.GetIdByEmailAsync(email);

      var emailExists = userId != Guid.Empty;
      return emailExists;
   }
}