using IAM.Application.Contracts;
using IAM.Domain;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;
using IAM.Domain.Entities;
using IAM.Domain.Interfaces;
using IAM.Domain.Mappers;
using IAM.Domain.Messages.Errors;
using IAM.Domain.Messages.Info;
using IAM.Domain.QueryRepositories;
using IAM.Domain.Repositories;
using Isopoh.Cryptography.Argon2;
using Myce.Response;

namespace IAM.Application.Services;

public class UserService : BaseService, IUserService
{
   private readonly IUnitOfWork _unitOfWork;
   private readonly IUserValidator _userValidator;
   private readonly IUserRepository _userRepository;
   private readonly IUserQueryRepository _userQueryRepository;

   public UserService(
       IUnitOfWork unitOfWork,
       IUserContext userContext,
       IUserValidator userValidator,
       IUserRepository userRepository,
       IUserQueryRepository userQueryRepository) : base(userContext)
   {
      _unitOfWork = unitOfWork;
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
      return await ExecuteIfUserOwnsAsync(request.CustomerId, async () =>
      {
         bool emailExists = await EmailExistsAsync(request.Email);

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
      });
   }

   public async Task<Result> UpdateAsync(Guid id, UserUpdateRequest request)
   {
      var user = await _userRepository.GetByIdAsync(id);
      return await ExecuteIfUserOwnsAsync(user?.CustomerId, async () =>
      {    
         var validator = _userValidator.ValidateUpdate(user?.Id, request);
         if (validator.HasError)
         {
            return Result.Failure(validator.Messages);
         }

         user.Update(request.Name, request.IsActive);

         return await CommitUpdateAsync(user);
      });
   }

   public async Task<Result> UpdatePasswordAsync(Guid id, UserUpdatePasswordRequest request)
   {
      var user = await _userRepository.GetByIdAsync(id);

      var validator = _userValidator.ValidateUpdatePassword(user, request);
      if (validator.HasError)
      {//todo validate the logged in user is the same as the user being updated  
         return Result.Failure(validator.Messages);
      }
      
      user.UpdatePassword(Argon2.Hash(request.PasswordNew));

      return await CommitUpdateAsync(user);
   }

   public async Task<Result> DeleteAsync(Guid id)
   {
      var user = await _userRepository.GetByIdAsync(id);

      return await ExecuteIfUserOwnsAsync(user?.CustomerId, async () =>
      {
         if (user == null)
         {
            return Result.Failure(new NotFoundError(Const.Entity.User));
         }

         await _unitOfWork.Users.DeleteAsync(id);
         await _unitOfWork.SaveChangesAsync();
         return Result.Success(new SuccessInfo());
      });
   }

   public async Task<Result> UpdateLastLoginAsync(Guid id)
   {
      var user = await _userRepository.GetByIdAsync(id);

      user.UpdateLastLogin();

      return await CommitUpdateAsync(user);
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

   private async Task<Result> CommitUpdateAsync(User user)
   {
      _unitOfWork.Users.Update(user);
      await _unitOfWork.SaveChangesAsync();

      return Result.Success(new SuccessInfo());
   }
}