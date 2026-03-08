using FluentAssertions;
using IAM.Application.Contracts;
using IAM.Application.Services;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.Entities;
using IAM.Domain.Messages;
using IAM.Domain.Messages.Errors;
using IAM.Domain.QueryRepositories;
using IAM.Domain.Repositories;
using Myce.Response;
using NSubstitute;

namespace IAM.Application.Tests.Services;

public class UserServiceTests
{
   private readonly IUnitOfWork _unitOfWork;
   private readonly IUserValidator _userValidator;
   private readonly IUserRepository _userRepository;
   private readonly IUserQueryRepository _userQueryRepository;
   private readonly UserService _sut;

   public UserServiceTests()
   {
      // No NSubstitute, você cria o mock diretamente como a interface
      _unitOfWork = Substitute.For<IUnitOfWork>();
      _userValidator = Substitute.For<IUserValidator>();
      _userRepository = Substitute.For<IUserRepository>();
      _userQueryRepository = Substitute.For<IUserQueryRepository>();

      // Configura a propriedade Users do Unit of Work para retornar o repositório mockado
      _unitOfWork.Users.Returns(_userRepository);

      _sut = new UserService(
          _unitOfWork,
          _userValidator,
          _userRepository,
          _userQueryRepository);
   }

   [Fact]
   public async Task CreateUserAsync_ShouldReturnForbiddenCustomerError_WhenOperatorIdDoesNotMatch()
   {
      var request = new UserCreateRequest(string.Empty, "test@test.com" , string.Empty, Guid.NewGuid());
      var differentOperatorId = Guid.NewGuid();

      var result = await _sut.CreateUserAsync(request, differentOperatorId, true, false);

      result.IsValid.Should().BeFalse();
      result.Messages.Should().ContainSingle(m => m is ForbiddenCustomerError);

      await _unitOfWork.DidNotReceive().SaveChangesAsync();
   }

   [Fact]
   public async Task CreateUserAsync_ShouldReturnValidationErrors_WhenValidatorFails()
   {
      var customerId = Guid.NewGuid();
      var request = new UserCreateRequest("John Smith", "test@test.com", string.Empty, customerId);

      _userValidator.ValidateCreate(request, true, true)
          .Returns(Result.Failure(new EmailAlreadyExistError(request.Email)));

      var result = await _sut.CreateUserAsync(request, customerId, true, true);

      result.IsValid.Should().BeFalse();
      result.Messages.Should().Contain(m => m is EmailAlreadyExistError);
      await _unitOfWork.DidNotReceive().SaveChangesAsync();
   }

   [Fact]
   public async Task CreateUserAsync_ShouldSaveUser_WhenRequestIsValid()
   {
      var customerId = Guid.NewGuid();
      var request = new UserCreateRequest("John Doe", "new@test.com", "SecurePassword123", customerId);

      _userValidator.ValidateCreate(request, true, false)
          .Returns(Result.Success());

      var result = await _sut.CreateUserAsync(request, customerId, true, false);

      result.IsValid.Should().BeTrue();

      // Verificação: Recebeu uma chamada de AddAsync e uma de SaveChanges
      await _unitOfWork.Users.Received(1).AddAsync(Arg.Any<User>());
      await _unitOfWork.Received(1).SaveChangesAsync();
   }

   [Fact]
   public async Task DeleteAsync_ShouldReturnNotFoundError_WhenUserDoesNotExist()
   {
      var userId = Guid.NewGuid();
      _userRepository.GetByIdAsync(userId).Returns((User)null);

      var result = await _sut.DeleteAsync(userId);

      result.IsValid.Should().BeFalse();
      result.Messages.Should().ContainSingle(m => m is NotFoundError);

      await _unitOfWork.Users.DidNotReceive().DeleteAsync(Arg.Any<Guid>());
   }
}