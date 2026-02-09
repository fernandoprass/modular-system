using IAM.Core.Services;
using IAM.Domain.DTOs.Responses;
using IAM.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Myce.Response;
using Myce.Response.Messages;

namespace IAM.API.Controllers;

[Route("api/[controller]")]
public class CustomerController : BaseController
{
   private readonly ICustomerService _customerService;

   public CustomerController(ICustomerService customerService)
   {
      _customerService = customerService;
   }

   [HttpGet("{id}")]
   public async Task<IActionResult> GetById(Guid id)
   {
      var customer = await _customerService.GetByIdAsync(id);
      return OkOrNotFound(customer);
   }

   [HttpGet("by-name/{name}")]
   public async Task<IActionResult> GetByName(string name)
   {
      var customer = await _customerService.GetByNameAsync(name);
      return OkOrNotFound(customer);
   }

   [HttpPost]
   public async Task<IActionResult> Create([FromBody] Customer customer)
   {
      if (!ModelState.IsValid)
      {
         // Consistent error envelope even for validation failures
         var errorResult = Result.Failure(new ErrorMessage("INVALID_CUSTOMER_DATA", "The customer data is invalid."));
         return BadRequest(errorResult);
      }

      var createdCustomer = await _customerService.CreateAsync(customer);
      var result = Result<Customer>.Success(createdCustomer);

      return CreatedAtAction(nameof(GetById), new { id = createdCustomer.Id }, result);
   }

   [HttpPut("{id}")]
   public async Task<IActionResult> Update(Guid id, [FromBody] Customer customer)
   {
      return await ExecuteIfExistsAsync(
          () => _customerService.ExistsAsync(id),
          async (exists) => await _customerService.UpdateAsync(customer));
   }

   [HttpDelete("{id}")]
   public async Task<IActionResult> Delete(Guid id)
   {
      return await ExecuteIfExistsAsync(
          () => _customerService.ExistsAsync(id),
          async (exists) => await _customerService.DeleteAsync(id));
   }
}