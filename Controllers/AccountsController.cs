using FinanceApi.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    [HttpGet("error")]
    public IActionResult GetError()
    {
        throw new NotImplementedException("This endpoint is not yet implemented");
    }

    [HttpGet("notfound")]
    public IActionResult GetNotFound()
    {
        throw new KeyNotFoundException("The requested account was not found");
    }

    [HttpGet("invalid")]
    public IActionResult GetInvalid()
    {
        throw new ArgumentException("Invalid account parameters provided");
    }

    [HttpGet]
    public IActionResult GetAccounts()
    {
        return Ok(new { Message = "This endpoint works correctly" });
    }

    // Task 4: custom exception types
    [HttpGet("{id}")]
    public IActionResult GetAccount(int id)
    {
        if (id <= 0)
        {
            throw new ValidationException(
                "Validation failed for the request.",
                new Dictionary<string, string[]>
                {
                    ["id"] = new[] { "Account id must be a positive integer." }
                });
        }

        if (id > 1000)
        {
            throw new NotFoundException($"Account with id {id} was not found.");
        }

        return Ok(new { id, balance = 1000.00m, owner = "Sample Owner" });
    }
}
