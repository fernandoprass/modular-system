using Asp.Versioning;
using IAM.Application.Contracts;
using IAM.Domain.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IAM.API.Controllers;

[ApiVersion(1)]
[Route("api/v{version:apiVersion}/iam/customers")]
public class CustomerController(
   ICustomerService customerService,
   IRegisterOrchestrator registerOrchestrator) : BaseController
{
   private readonly ICustomerService _customerService = customerService;
   private readonly IRegisterOrchestrator _registerOrchestrator = registerOrchestrator;

   [HttpGet("{id}")]
   [Authorize]
   public async Task<IActionResult> GetById(Guid id)
   {
      var customer = await _customerService.GetByIdAsync(id);
      return OkOrNotFound(customer);
   }

   [HttpGet()]
   [Authorize]
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
   [Authorize]
   public async Task<IActionResult> Update(Guid id, [FromBody] CustomerUpdateRequest customer)
   {
      var result = await _customerService.UpdateAsync(id, customer);
      return OkOrNotFound(result);
   }

   [HttpPatch("{id}/code")]
   [Authorize]
   public async Task<IActionResult> UpdateCode(Guid id, [FromBody] CustomerUpdateCodeRequest customer)
   {
      var result = await _customerService.UpdateCodeAsync(id, customer);
      return OkOrNotFound(result);
   }

   [HttpDelete("{id}")]
   [Authorize]
   public async Task<IActionResult> Delete(Guid id)
   {
      var result = await _registerOrchestrator.DeleteCustomerAsync(id);
      return OkOrNotFound(result);
   }
}