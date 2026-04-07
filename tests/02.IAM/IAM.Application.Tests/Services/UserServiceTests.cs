using FluentAssertions;
using IAM.Application.Contracts;
using IAM.Application.Services;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.Entities;
using IAM.Domain.Interfaces;
using IAM.Domain.Messages;
using IAM.Domain.QueryRepositories;
using IAM.Domain.Repositories;
using Myce.Response;
using NSubstitute;
using Shared.Application.Contracts;

namespace IAM.Application.Tests.Services;

public class UserServiceTests
{
   private readonly IIamUnitOfWork _unitOfWorkMock;
   private readonly IUserValidator _userValidatorMock;
   private readonly IUserContext _userContextMock;
   private readonly IUserRepository _userRepositoryMock;
   private readonly IUserQueryRepository _userQueryRepositoryMock;
   private readonly UserService _userService;

   public UserServiceTests()
   {
      _unitOfWorkMock = Substitute.For<IIamUnitOfWork>();
      _userContextMock = Substitute.For<IUserContext>();
      _userValidatorMock = Substitute.For<IUserValidator>();
      _userRepositoryMock = Substitute.For<IUserRepository>();
      _userQueryRepositoryMock = Substitute.For<IUserQueryRepository>();
      
      _unitOfWorkMock.Users.Returns(_userRepositoryMock);
      _userContextMock.UserOwnerId.Returns(Guid.CreateVersion7());

      _userService = new UserService(
          _unitOfWorkMock,
          _userContextMock,
          _userValidatorMock,
          _userRepositoryMock,
          _userQueryRepositoryMock);
   }

   [Fact]
   public async Task CreateUserAsync_ShouldReturnForbiddenCustomerError_WhenOperatorIdDoesNotMatch()
   {
      var request = new UserCreateRequest(string.Empty, "test@test.com" , string.Empty, Guid.NewGuid());

      var result = await _userService.CreateUserAsync(request, true);

      result.IsSuccess.Should().BeFalse();
      result.Messages.Should().ContainSingle(m => m is ForbiddenCustomerError);

      await _unitOfWorkMock.DidNotReceive().SaveChangesAsync();
   }

   [Fact]
   public async Task CreateUserAsync_ShouldReturnValidationErrors_WhenValidatorFails()
   {
      var request = new UserCreateRequest("John Smith", "test@test.com", string.Empty, _userContextMock.UserOwnerId);


      _userQueryRepositoryMock.GetIdByEmailAsync(request.Email).Returns(Guid.NewGuid());

      _userValidatorMock.ValidateCreate(request,customerExists: true, emailAlreadyExists: true)
          .Returns(Result.Failure(new EmailAlreadyExistError(request.Email)));

      var result = await _userService.CreateUserAsync(request, true);

      result.IsSuccess.Should().BeFalse();
      result.Messages.Should().Contain(m => m is EmailAlreadyExistError);
      await _unitOfWorkMock.DidNotReceive().SaveChangesAsync();
   }

   [Fact]
   public async Task CreateUserAsync_ShouldSaveUser_WhenRequestIsValid()
   {
      var request = new UserCreateRequest("John Doe", "new@test.com", "SecurePassword123", _userContextMock.UserOwnerId);

      _userQueryRepositoryMock.GetIdByEmailAsync(request.Email).Returns(Guid.Empty);

      _userValidatorMock.ValidateCreate(request, customerExists: true, emailAlreadyExists: false).Returns(Result.Success());

      var result = await _userService.CreateUserAsync(request, true);

      result.IsSuccess.Should().BeTrue();

      await _unitOfWorkMock.Users.Received(1).AddAsync(Arg.Any<User>());
      await _unitOfWorkMock.Received(1).SaveChangesAsync();
   }

   [Fact]
   public async Task DeleteAsync_ShouldReturnForbiddenCustomerError_EvenWhenUserDoesNotExist()
   {
      var userId = Guid.NewGuid();
      _userRepositoryMock.GetByIdAsync(userId).Returns((User)null);

      var result = await _userService.DeleteAsync(userId);

      result.IsSuccess.Should().BeFalse();
      result.Messages.Should().ContainSingle(m => m is ForbiddenCustomerError);

      await _unitOfWorkMock.Users.DidNotReceive().DeleteAsync(Arg.Any<Guid>());
   }
}