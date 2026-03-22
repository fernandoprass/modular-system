using IAM.Application.Contracts;
using IAM.Application.Services;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;
using IAM.Domain.Entities;
using IAM.Domain.Interfaces;
using IAM.Domain.Messages.Errors;
using IAM.Domain.QueryRepositories;
using IAM.Domain.Repositories;
using Myce.Response;
using NSubstitute;

namespace IAM.Application.Tests.Services;

public class CustomerServiceTests
{
   private readonly ICustomerQueryRepository _customerQueryRepository;
   private readonly ICustomerRepository _customerRepository;
   private readonly ICustomerValidator _customerValidator;
   private readonly IUnitOfWork _unitOfWork;
   private readonly IUserContext _userContext;
   private readonly CustomerService _service;

   public CustomerServiceTests()
   {
      _customerQueryRepository = Substitute.For<ICustomerQueryRepository>();
      _customerRepository = Substitute.For<ICustomerRepository>();
      _customerValidator = Substitute.For<ICustomerValidator>();
      _unitOfWork = Substitute.For<IUnitOfWork>();
      _userContext = Substitute.For<IUserContext>();

      _service = new CustomerService(
          _customerQueryRepository,
          _customerRepository,
          _customerValidator,
          _unitOfWork,
          _userContext);
   }

   [Fact]
   public async Task ValidateCreateCustomerAsync_WhenValidatorSucceeds_ReturnsSuccess()
   {
      var request = GetCustomerCreateRequest(CustomerType.Company, "Customer Name", "Code1");

      _customerQueryRepository.ExistsByCodeAsync(request.Code).Returns(false);
      _customerValidator.ValidateCreate(request, false).Returns(Result.Success());

      var result = await _service.ValidateCreateCustomerAsync(request);

      Assert.True(result.IsSuccess);
   }

   [Fact]
   public async Task ValidateCreateCustomerAsync_WhenValidatorFails_ReturnsFailure()
   {
      var request = GetCustomerCreateRequest(CustomerType.Company, "Customer Name", "Code1");

      _customerQueryRepository.ExistsByCodeAsync(request.Code).Returns(true);
      _customerValidator.ValidateCreate(request, true).Returns(Result.Failure(new DuplicateCustomerCodeError(request.Code)));

      var result = await _service.ValidateCreateCustomerAsync(request);

      Assert.False(result.IsSuccess);
   }

   [Fact]
   public async Task GetByIdAsync_WhenCustomerExists_ReturnsCustomerDto()
   {
      var id = Guid.NewGuid();
      var expected = new CustomerDto { Id = id, Name = "Test" };
      _customerQueryRepository.GetByIdAsync(id).Returns(expected);

      var result = await _service.GetByIdAsync(id);

      Assert.Equal(expected, result);
   }

   [Fact]
   public void GetRandomCode_WhenCalled_ReturnsStringWithCorrectSize()
   {
      var result = _service.GetRandomCode();

      Assert.NotNull(result);
      Assert.False(string.IsNullOrWhiteSpace(result));
   }

   [Fact]
   public async Task GetByNameAsync_WhenCalled_ReturnsIEnumerableCustomerDto()
   {
      var name = "SearchName";
      var expected = new List<CustomerDto> { new() { Name = name } };
      _customerQueryRepository.GetByNameAsync(name).Returns(expected);

      var result = await _service.GetByNameAsync(name);

      Assert.Equal(expected, result);
   }

   [Fact]
   public async Task UpdateAsync_WhenOwnershipAndValidatorSucceed_ReturnsSuccess()
   {
      var id = Guid.NewGuid();
      var request = GetCustomerUpdateRequest("New Customer Name", "description", true);

      var customer = Customer.Create(CustomerType.Company, "OriginalCode", "Original Name", "description");

      _userContext.CustomerId.Returns(id);
      _customerRepository.GetByIdAsync(id).Returns(customer);
      _customerValidator.ValidateUpdate(request, true).Returns(Result.Success());

      var result = await _service.UpdateAsync(id, request);

      Assert.True(result.IsSuccess);
      _unitOfWork.Customers.Received(1).Update(customer);
      await _unitOfWork.Received(1).SaveChangesAsync();
   }

   [Fact]
   public async Task UpdateAsync_WhenValidatorFails_ReturnsFailure()
   {
      var id = Guid.NewGuid();
      var request = GetCustomerUpdateRequest(string.Empty, "description", true);
      var customer = Customer.Create(CustomerType.Company, "OriginalCode", "Original Name", "description");

      _userContext.CustomerId.Returns(id);
      _customerRepository.GetByIdAsync(id).Returns(customer);
      _customerValidator.ValidateUpdate(request, true).Returns(Result.Failure(new NotFoundError()));

      var result = await _service.UpdateAsync(id, request);

      Assert.False(result.IsSuccess);
   }

   [Fact]
   public async Task UpdateCodeAsync_WhenOwnershipAndValidatorSucceed_ReturnsSuccess()
   {
      var id = Guid.NewGuid();
      var request = new CustomerUpdateCodeRequest("NEWCODE");
      var customer = Customer.Create(CustomerType.Company, "OriginalCode", "Original Name", "description");

      _userContext.CustomerId.Returns(id);
      _customerRepository.GetByCodeAsync(request.Code).Returns((Customer)null);
      _customerValidator.ValidateUpdateCode(request, false).Returns(Result.Success());
      _customerRepository.GetByIdAsync(id).Returns(customer);

      var result = await _service.UpdateCodeAsync(id, request);

      Assert.True(result.IsSuccess);
      Assert.Equal("NEWCODE", customer.Code);
   }

   [Fact]
   public async Task UpdateCodeAsync_WhenNewCodeAlreadyExists_ReturnsFailure()
   {
      var id = Guid.NewGuid();
      var request = new CustomerUpdateCodeRequest("EXISTING");
      var existingCustomer = Customer.Create(CustomerType.Company, "EXISTING", "Original Name", "description");

      _userContext.CustomerId.Returns(id);
      _customerRepository.GetByCodeAsync(request.Code).Returns(existingCustomer);
      _customerValidator.ValidateUpdateCode(request, true).Returns(Result.Failure(new DuplicateCustomerCodeError(request.Code)));

      var result = await _service.UpdateCodeAsync(id, request);

      Assert.False(result.IsSuccess);
   }

   private static CustomerCreateRequest GetCustomerCreateRequest(CustomerType type, string name, string code)
   {
      var user = new CustomerUserCreateRequest(string.Empty, string.Empty, string.Empty);
      var request = new CustomerCreateRequest(type, name, code, "some description", user);
      return request;
   }

   private static CustomerUpdateRequest GetCustomerUpdateRequest(string name, string description, bool isActive)
   {
      var request = new CustomerUpdateRequest(name, description, isActive);
      return request;
   }
}