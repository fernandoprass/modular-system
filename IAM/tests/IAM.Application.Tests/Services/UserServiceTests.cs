using IAM.Application.Contracts;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.QueryRepositories;
using IAM.Domain.Repositories;
using Moq;

namespace IAM.Application.Services.Tests;

public class UserServiceTests
{
   private readonly Mock<IUnitOfWork> _unitOfWorkMock;
   private readonly Mock<IUserRepository> _userRepositoryMock;
   private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;
   private readonly Mock<IUserValidator> _userFluentValidatorMock;
   private readonly UserService _userService;

   public UserServiceTests()
   {
      _unitOfWorkMock = new Mock<IUnitOfWork>();
      _userRepositoryMock = new Mock<IUserRepository>();
      _userQueryRepositoryMock = new Mock<IUserQueryRepository>();
      _userFluentValidatorMock = new Mock<IUserValidator>();
    
      _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepositoryMock.Object);

      _userService = new UserService(
          _unitOfWorkMock.Object,
          _userFluentValidatorMock.Object,
          _userRepositoryMock.Object,
          _userQueryRepositoryMock.Object
          );
   }

   [Fact]
   public async Task CreateUserAsync_ShouldCallValidator()
   {
      //todo: fix unit test
      //// Arrange
      //var request = new CreateUserRequest
      //{
      //   Name = "Test User",
      //   Email = "test@example.com",
      //   Password = "password123",
      //   CustomerId = Guid.NewGuid()
      //};

      //_userQueryRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync((UserDto?)null);

      //// Act
      //await _userService.CreateUserAsync(request);

      //// Assert
      //_userValidatorMock.Verify(v => v.ValidateCreateUser(request), Times.Once);
      //_unitOfWorkMock.Verify(u => u.Users.AddAsync(It.IsAny<User>()), Times.Once);
      //_unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
   }

   [Fact]
   public async Task ValidateCredentialsAsync_ShouldCallValidator()
   {
      // Arrange
      var email = "test@example.com";
      var password = "password";

      // Act
      await _userService.ValidateCredentialsAsync(email, password);

      // Assert
      _userValidatorMock.Verify(v => v.ValidateCredentials(email, password), Times.Once);
   }

   [Fact]
   public async Task UpdatePasswordAsync_ShouldCallValidator()
   {
      // Arrange
      var request = new UserUpdatePasswordRequest
      {
         Email = "test@example.com",
         PasswordOld = "old",
         PasswordNew = "new"
      };

      // Act
      await _userService.UpdatePasswordAsync(request);

      // Assert
      _userValidatorMock.Verify(v => v.Validate(request), Times.Once);
   }
}
