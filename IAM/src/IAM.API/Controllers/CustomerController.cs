using Asp.Versioning;
using IAM.Application.Contracts;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Myce.Response;
using Myce.Response.Messages;

namespace IAM.API.Controllers;

[ApiVersion(1)]
[Route("api/v{version:apiVersion}/iam/customers")]
public class CustomerController : BaseController
{
   private readonly ICustomerService _customerService;
   private readonly IRegisterOrchestrator _registerOrchestrator;

   public CustomerController(
      ICustomerService customerService,
      IRegisterOrchestrator registerOrchestrator)
   {
      _customerService = customerService;
      _registerOrchestrator = registerOrchestrator;
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
   public async Task<IActionResult> Create([FromBody] CustomerCreateRequest customer)
   {
      var result = await _registerOrchestrator.RegisterCustomerAsync(customer);
      return OkOrNotFound(result);
   }

   [HttpPut("{id}")]
   public async Task<IActionResult> Update(Guid id, [FromBody] CustomerUpdateRequest customer)
   {
      var result = await _customerService.UpdateAsync(id, customer);
      return OkOrNotFound(result);
   }

   [HttpDelete("{id}")]
   public async Task<IActionResult> Delete(Guid id)
   {
      var result = await _customerService.DeleteAsync(id);
      return OkOrNotFound(result);
   }
}