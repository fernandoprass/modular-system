using IAM.Application.Validators;
using IAM.Domain;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.Entities;
using IAM.Domain.Messages;
using IAM.Domain.Messages.Errors;
using Isopoh.Cryptography.Argon2;


namespace IAM.Application.Tests.Validators;

public class UserValidatorTests
{
   private readonly UserValidator _validator;

   public UserValidatorTests()
   {
      _validator = new UserValidator();
   }

   [Fact]
   public void ValidateCreate_ShouldBeSuccess_WhenAllDataIsValid()
   {
      var request = new UserCreateRequest("Dev Senior", "test@example.com", "Strong#Pass123", Guid.NewGuid());

      var result = _validator.ValidateCreate(request, emailAlreadyExists: false, customerExists: true);

      Assert.True(result.IsValid);
   }

   [Fact]
   public void ValidateCreate_ShouldHaveError_WhenEmailAlreadyExists()
   {
      var request = new UserCreateRequest("Dev Senior", "exists@example.com", "Strong#Pass123", Guid.NewGuid());

      var result = _validator.ValidateCreate(request, emailAlreadyExists: true, customerExists: true);

      Assert.False(result.IsValid);
      Assert.Contains(result.Messages, m => m is EmailAlreadyExistError);
   }

   [Fact]
   public void ValidateCreate_ShouldHaveError_WhenCustomerDoesNotExist()
   {
      var request = new UserCreateRequest("Dev Senior", "test@example.com", "Strong#Pass123", Guid.NewGuid());

      var result = _validator.ValidateCreate(request, emailAlreadyExists: false, customerExists: false);

      Assert.False(result.IsValid);
      Assert.Contains(result.Messages, m => m is NotFoundError && m.Show().Contains(Const.Entity.Customer));
   }

   [Theory]
   [InlineData("1234567")] // Min length
   [InlineData("nospecialchar123")] // Missing special and upper
   [InlineData("Onlyupper#")] // Missing digit and lower
   public void ValidateCreate_ShouldHaveError_WhenPasswordIsWeak(string weakPassword)
   {
      var request = new UserCreateRequest("Dev Senior", "test@example.com", weakPassword, Guid.NewGuid());

      var result = _validator.ValidateCreate(request, false, true);

      Assert.False(result.IsValid);
   }

   [Fact]
   public void ValidateUpdate_ShouldHaveError_WhenIdIsNull()
   {
      var request = new UserUpdateRequest("New Name", true);

      var result = _validator.ValidateUpdate(null, request);

      Assert.False(result.IsValid);
      Assert.Contains(result.Messages, m => m is NotFoundError);
   }

   [Fact]
   public void ValidateUpdatePassword_ShouldBeSuccess_WhenCredentialsAreValid()
   {
      var oldPassword = "Old#Password123";
      var user = User.Create("User Test", "test@email.com", Argon2.Hash(oldPassword), Guid.NewGuid());
      var request = new UserUpdatePasswordRequest(user.Email, oldPassword, "New#StrongPass88");

      var result = _validator.ValidateUpdatePassword(user, request);

      Assert.True(result.IsValid);
   }

   [Fact]
   public void ValidateUpdatePassword_ShouldHaveError_WhenOldPasswordIsIncorrect()
   {
      var user = User.Create("User Test", "test@email.com", Argon2.Hash("Correct#123"), Guid.NewGuid());
      var request = new UserUpdatePasswordRequest(user.Email, "Wrong#123", "New#StrongPass88");

      var result = _validator.ValidateUpdatePassword(user, request);

      Assert.False(result.IsValid);
      Assert.Contains(result.Messages, m => m is PasswordNotValidError);
   }

   [Fact]
   public void ValidateUpdatePassword_ShouldHaveError_WhenUserNotFound()
   {
      var request = new UserUpdatePasswordRequest("nonexistent@email.com", "any", "New#StrongPass88");

      var result = _validator.ValidateUpdatePassword(null, request);

      Assert.False(result.IsValid);
      Assert.Contains(result.Messages, m => m is NotFoundError);
   }
}

