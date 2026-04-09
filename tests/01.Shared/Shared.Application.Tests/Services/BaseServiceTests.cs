using Myce.Response;
using NSubstitute;
using Shared.Application.Contracts;
using Shared.Application.Services;
using Shared.Domain.Messages;

namespace Shared.Application.Tests.Services;

public class TestService : BaseService
{
   public TestService(IUserContext userContext) : base(userContext) { }

   public async Task<Result> TestExecuteIfUserOwnsAsync(Guid? resourceCustomerId, Func<Task<Result>> action)
       => await ExecuteIfUserOwnsAsync(resourceCustomerId, action);

   public async Task<TResult> TestExecuteIfUserOwnsAsyncGeneric<TResult>(Guid? resourceCustomerId, Func<Task<TResult>> action) where TResult : Result
       => await ExecuteIfUserOwnsAsync<TResult>(resourceCustomerId, action);
}

public class BaseServiceTests
{
   private readonly IUserContext _userContextMock;
   private readonly TestService _service;

   public BaseServiceTests()
   {
      _userContextMock = Substitute.For<IUserContext>();
      _service = new TestService(_userContextMock);
   }

   [Fact]
   public async Task ExecuteIfUserOwnsAsync_ShouldAllowForDifferentCustomer_WhenUserIsSystemAdmin()
   {
      _userContextMock.IsSystemAdmin.Returns(true);
      var resourceCustomerId = Guid.NewGuid();
      var actionCalled = false;

      var result = await _service.TestExecuteIfUserOwnsAsync(resourceCustomerId, () =>
      {
         actionCalled = true;
         return Task.FromResult(Result.Success());
      });

      Assert.True(result.IsSuccess);
      Assert.True(actionCalled);
   }

   [Fact]
   public async Task ExecuteIfUserOwnsAsync_ShouldAllow_WhenUserOwnsTheResource()
   {
      var myCustomerId = Guid.NewGuid();
      _userContextMock.IsSystemAdmin.Returns(false);
      _userContextMock.UserOwnerId.Returns(myCustomerId);

      var actionCalled = false;

      var result = await _service.TestExecuteIfUserOwnsAsync(myCustomerId, () =>
      {
         actionCalled = true;
         return Task.FromResult(Result.Success());
      });

      Assert.True(result.IsSuccess);
      Assert.True(actionCalled);
   }

   [Fact]
   public async Task ExecuteIfUserOwnsAsync_ShouldFail_WhenUserIsNotOwnerAndNotAdmin()
   {
      _userContextMock.IsSystemAdmin.Returns(false);
      _userContextMock.UserOwnerId.Returns(Guid.NewGuid());

      var targetOwnerId = Guid.NewGuid(); // Different id
      var actionCalled = false;

      var result = await _service.TestExecuteIfUserOwnsAsync(targetOwnerId, () =>
      {
         actionCalled = true;
         return Task.FromResult(Result.Success());
      });

      Assert.False(result.IsSuccess);
      Assert.False(actionCalled);
      Assert.IsType<UnauthorizedAccessError>(result.Messages.First());
   }

   [Fact]
   public async Task ExecuteIfUserOwnsAsync_Generic_ShouldReturnCorrectTypeOnFailure()
   {
      _userContextMock.IsSystemAdmin.Returns(false);
      _userContextMock.UserOwnerId.Returns(Guid.NewGuid());

      var result = await _service.TestExecuteIfUserOwnsAsyncGeneric<Result<string>>(Guid.NewGuid(), () =>
          Task.FromResult(Result<string>.Success("Should not be called")));

      Assert.False(result.IsSuccess);
      Assert.IsType<Result<string>>(result);
      Assert.IsType<UnauthorizedAccessError>(result.Messages.First());
   }
}
