using FluentAssertions;
using Myce.Response;
using NSubstitute;
using Shared.Application.Contracts;
using Shared.Application.Services;
using Shared.Domain.DTOs.Requests;
using Shared.Domain.DTOs.Responses;
using Shared.Domain.Entities;
using Shared.Domain.Enums;
using Shared.Domain.Interfaces;
using Shared.Domain.Messages;

namespace Shared.Application.Tests.Services;

public class ParameterServiceTests
{
   private readonly ISharedUnitOfWork _unitOfWorkMock;
   private readonly IUserContext _userContextMock;
   private readonly IParameterValidator _parameterValidatorMock;
   private readonly IParameterRepository _parameterRepositoryMock;
   private readonly IParameterOverrideRepository _parameterOverrideRepositoryMock;
   private readonly IParameterQueryRepository _parameterQueryRepositoryMock;
   private readonly ParameterService _parameterService;

   public ParameterServiceTests()
   {
      _unitOfWorkMock = Substitute.For<ISharedUnitOfWork>();
      _userContextMock = Substitute.For<IUserContext>();
      _parameterValidatorMock = Substitute.For<IParameterValidator>();
      _parameterRepositoryMock = Substitute.For<IParameterRepository>();
      _parameterOverrideRepositoryMock = Substitute.For<IParameterOverrideRepository>();
      _parameterQueryRepositoryMock = Substitute.For<IParameterQueryRepository>();

      _userContextMock.UserOwnerId.Returns(Guid.NewGuid());
      _userContextMock.UserId.Returns(Guid.NewGuid());

      _parameterService = new ParameterService(
          _unitOfWorkMock,
          _userContextMock,
          _parameterValidatorMock,
          _parameterRepositoryMock,
          _parameterOverrideRepositoryMock,
          _parameterQueryRepositoryMock);
   }

   [Fact]
   public async Task GetByIdAsync_ShouldReturnNotFoundError_WhenParameterDoesNotExist()
   {
      var parameterId = Guid.NewGuid();
      _parameterQueryRepositoryMock.GetByIdAsync(parameterId).Returns((ParameterDto)null);

      var result = await _parameterService.GetByIdAsync(parameterId);

      result.IsSuccess.Should().BeFalse();
      result.Messages.Should().ContainSingle(m => m is NotFoundError);
   }

   [Fact]
   public async Task CreateAsync_ShouldReturnValidationErrors_WhenValidatorFails()
   {
      var request = new ParameterCreateRequest("Module", "Group", "Name", "Title", "Desc", ParameterType.String, "Value", ParameterOverrideType.None, true);
      _parameterQueryRepositoryMock.GetByModuleGroupAndKeyAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(new ParameterDto());
      _parameterValidatorMock.ValidateCreate(request, true).Returns(Result.Failure(new ParameterDuplicatedError("M", "G", "K")));

      var result = await _parameterService.CreateAsync(request);

      result.IsSuccess.Should().BeFalse();
      result.Messages.Should().Contain(m => m is ParameterDuplicatedError);
      await _unitOfWorkMock.DidNotReceive().SaveChangesAsync();
   }

   [Fact]
   public async Task CreateAsync_ShouldSaveParameter_WhenRequestIsValid()
   {
      var request = new ParameterCreateRequest("Module", "Group", "Name", "Title", "Desc", ParameterType.String, "Value", ParameterOverrideType.None, true);
      _parameterQueryRepositoryMock.GetByModuleGroupAndKeyAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns((ParameterDto)null);
      _parameterValidatorMock.ValidateCreate(request, false).Returns(Result.Success());

      var result = await _parameterService.CreateAsync(request);

      result.IsSuccess.Should().BeTrue();
      await _unitOfWorkMock.Parameters.Received(1).AddAsync(Arg.Any<Parameter>());
      await _unitOfWorkMock.Received(1).SaveChangesAsync();
   }

   [Fact]
   public async Task SaveOverrideValueAsync_ShouldCreateNewOverride_WhenOverrideDoesNotExist()
   {
      var parameterId = Guid.NewGuid();
      var request = new ParameterOwnerUpdateRequest("NewValue");
      var parameter = new Parameter { OverrideType = ParameterOverrideType.UserOwnerId };

      _parameterRepositoryMock.GetByIdAsync(parameterId).Returns(parameter);
      _parameterValidatorMock.ValidateOwnerUpdate(parameter, request).Returns(Result.Success());
      _parameterOverrideRepositoryMock.GetByParameterIdAndOwnerIdAsync(parameterId, _userContextMock.UserOwnerId).Returns((ParameterOverride)null);

      var result = await _parameterService.SaveOverrideValueAsync(parameterId, request);

      result.IsSuccess.Should().BeTrue();
      await _unitOfWorkMock.ParameterOverrides.Received(1).AddAsync(Arg.Any<ParameterOverride>());
      await _unitOfWorkMock.Received(1).SaveChangesAsync();
   }

   [Fact]
   public async Task DeleteAsync_ShouldReturnNotFoundError_WhenParameterDoesNotExist()
   {
      var parameterId = Guid.NewGuid();
      _parameterRepositoryMock.GetByIdAsync(parameterId).Returns((Parameter)null);

      var result = await _parameterService.DeleteAsync(parameterId);

      result.IsSuccess.Should().BeFalse();
      result.Messages.Should().ContainSingle(m => m is NotFoundError);
      await _unitOfWorkMock.Parameters.DidNotReceive().DeleteAsync(Arg.Any<Guid>());
   }

   [Theory]
   [InlineData("not-an-int", "Int32")]
   [InlineData("12.5", "Int32")] 
   [InlineData("abc", "Decimal")]
   [InlineData("2023-13-40", "DateTime")] 
   [InlineData("invalid-date", "DateTime")]
   public async Task GetAndParseAsync_ShouldThrowInvalidDataException_WhenValueIsInvalid(string invalidValue, string typeName)
   {
      var key = "Module.Group.Key";
      var valueDto = new ParameterValueDto { Value = invalidValue };

      _parameterQueryRepositoryMock.GetValueAsync(key, _userContextMock.UserOwnerId, _userContextMock.UserId)
         .Returns(valueDto);

      Func<Task> act = typeName switch
      {
         "Int32" => async () => await _parameterService.GetIntAsync(key),
         "Decimal" => async () => await _parameterService.GetDecimalAsync(key),
         "DateTime" => async () => await _parameterService.GetDateTimeAsync(key),
         _ => throw new ArgumentException("Unsupported type in test")
      };

      await act.Should().ThrowAsync<InvalidDataException>()
         .WithMessage($"*'{invalidValue}'*invalid*'{key}'*type {typeName}*");
   }

   [Theory]
   [InlineData("true", true)]
   [InlineData("false", false)]
   public async Task GetBoolAsync_ShouldReturnParsedValue_WhenKeyExists(string value, bool expected)
   {
      var key = "Module.Group.Key";
      var valueDto = new ParameterValueDto { Value = value };
      _parameterQueryRepositoryMock.GetValueAsync(key, _userContextMock.UserOwnerId, _userContextMock.UserId).Returns(valueDto);

      var result = await _parameterService.GetBoolAsync(key);

      result.Should().Be(expected);
   }

   [Fact]
   public async Task GetIntAsync_ShouldThrowInvalidDataException_WhenValueIsInvalid()
   {
      var key = "Module.Group.Key";
      var valueDto = new ParameterValueDto { Value = "not-an-int" };
      _parameterQueryRepositoryMock.GetValueAsync(key, _userContextMock.UserOwnerId, _userContextMock.UserId).Returns(valueDto);

      Func<Task> act = async () => await _parameterService.GetIntAsync(key);

      await act.Should().ThrowAsync<InvalidDataException>();
   }

   [Fact]
   public async Task GetDecimalAsync_ShouldReturnParsedValue_WhenValueIsValid()
   {
      var key = "Finance.Tax.Rate";
      var valueDto = new ParameterValueDto { Value = "15.50" };
      _parameterQueryRepositoryMock.GetValueAsync(key, _userContextMock.UserOwnerId, _userContextMock.UserId).Returns(valueDto);

      var result = await _parameterService.GetDecimalAsync(key);

      result.Should().Be(15.50m);
   }

   [Fact]
   public async Task GetDateTimeAsync_ShouldReturnParsedValue_WhenValueIsValid()
   {
      var key = "System.Schedule.StartTime";
      var dateValue = "2023-12-25T10:00:00";
      var valueDto = new ParameterValueDto { Value = dateValue };

      _parameterQueryRepositoryMock.GetValueAsync(key, _userContextMock.UserOwnerId, _userContextMock.UserId).Returns(valueDto);

      var result = await _parameterService.GetDateTimeAsync(key);

      result.Should().Be(DateTime.Parse(dateValue));
   }

   [Fact]
   public async Task GetAndParseAsync_ShouldThrowArgumentNullException_WhenKeyDoesNotExist()
   {
      var key = "NonExistent.Key";
      _parameterQueryRepositoryMock.GetValueAsync(key, _userContextMock.UserOwnerId, _userContextMock.UserId).Returns((ParameterValueDto)null);

      Func<Task> act = async () => await _parameterService.GetStringAsync(key);

      await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("key");
   }

   [Fact]
   public async Task GetStringAsync_ShouldReturnEmptyString_WhenValueIsNull()
   {
      var key = "Module.Group.Key";
      var valueDto = new ParameterValueDto { Value = null };
      _parameterQueryRepositoryMock.GetValueAsync(key, _userContextMock.UserOwnerId, _userContextMock.UserId).Returns(valueDto);

      var result = await _parameterService.GetStringAsync(key);

      result.Should().BeEmpty();
   }
}
