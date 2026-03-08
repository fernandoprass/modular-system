using IAM.Domain;
using IAM.Domain.Messages.Errors;
using Microsoft.AspNetCore.Mvc;
using Myce.Response;

namespace IAM.API.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
   protected IActionResult OkOrNotFound<T>(T? value) where T : class
   {
      if (value == null)
      {
         return NotFound(Result<T>.Failure(new NotFoundError()));
      }

      return value is Result ? Ok(value) : Ok(Result<T>.Success(value));
   }

   protected async Task<IActionResult> ExecuteIfExistsAsync<T>(
       Func<Task<T?>> getEntityAsync,
       Func<T, Task> actionAsync)
   {
      var entity = await getEntityAsync();

      if (entity == null)
         return NotFound(Result.Failure(new NotFoundError()));

      await actionAsync(entity);
      return Ok(Result.Success("Operation executed successfully."));
   }
}