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
}
