using IAM.Core.Services;
using IAM.Domain.DTOs.Responses;
using IAM.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IAM.API.Controllers;

[Route("api/[controller]")]
public class CustomerController : BaseController
{
   private readonly ICustomerService _customerService;

   public CustomerController(ICustomerService customerService)
   {
      _customerService = customerService;
   }

   [HttpGet]
   public async Task<Ok<IEnumerable<CustomerDto>>> GetAll()
   {
      var customers = await _customerService.GetAllAsync();
      return TypedResults.Ok(customers);
   }

   [HttpGet("{id}")]
   public async Task<Results<Ok<CustomerDto>, NotFound>> GetById(Guid id)
   {
      var customer = await _customerService.GetByIdAsync(id);
      return OkOrNotFound(customer);
   }

   [HttpGet("by-name/{name}")]
   public async Task<Results<Ok<CustomerDto>, NotFound>> GetByName(string name)
   {
      var customer = await _customerService.GetByNameAsync(name);
      return OkOrNotFound(customer);
   }

   [HttpPost]
   public async Task<Results<CreatedAtRoute<Customer>, BadRequest<ModelStateDictionary>>> Create([FromBody] Customer customer)
   {
      if (!ModelState.IsValid)
      {
         return TypedResults.BadRequest(ModelState);
      }

      var createdCustomer = await _customerService.CreateAsync(customer);
      return TypedResults.CreatedAtRoute(createdCustomer, nameof(GetById), new { id = createdCustomer.Id });
   }

   [HttpPut("{id}")]
   public async Task<Results<NoContent, NotFound>> Update(Guid id, [FromBody] Customer customer)
   {
      return await ExecuteIfExistsAsync(
         () => _customerService.ExistsAsync(id),
         async () => await _customerService.UpdateAsync(customer));
   }

   [HttpDelete("{id}")]
   public async Task<Results<NoContent, NotFound>> Delete(Guid id)
   {
      return await ExecuteIfExistsAsync(
          () => _customerService.ExistsAsync(id),
          async () => await _customerService.DeleteAsync(id));
   }
}