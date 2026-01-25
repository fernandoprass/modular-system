using IAM.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace IAM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DatabaseController : ControllerBase
{
    private readonly IDatabaseSeeder _databaseSeeder;

    public DatabaseController(IDatabaseSeeder databaseSeeder)
    {
        _databaseSeeder = databaseSeeder;
    }

    [HttpPost("seed")]
    public async Task<IActionResult> SeedDatabase()
    {
        try
        {
            await _databaseSeeder.SeedAsync();
            return Ok(new { message = "Database seeded successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error seeding database", error = ex.Message });
        }
    }
}