using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace IAM.API.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected Results<Ok<T>, NotFound> OkOrNotFound<T>(T? value) where T : class
    {
        return value == null ? TypedResults.NotFound() : TypedResults.Ok(value);
    }

    protected async Task<Results<Ok, NotFound>> ExecuteIfExistsAsync<T>(
        Func<Task<T?>> getEntityAsync,
        Func<T, Task> actionAsync)
    {
        var entity = await getEntityAsync();
        if (entity == null)
        {
            return TypedResults.NotFound();
        }
        await actionAsync(entity);
        return TypedResults.Ok();
    }

    protected async Task<Results<NoContent, NotFound>> ExecuteIfExistsAsync(
        Func<Task<bool>> existsAsync,
        Func<Task> actionAsync)
    {
        if (!await existsAsync())
        {
            return TypedResults.NotFound();
        }
        await actionAsync();
        return TypedResults.NoContent();
    }
}