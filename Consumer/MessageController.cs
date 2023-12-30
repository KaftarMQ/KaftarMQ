using Microsoft.AspNetCore.Mvc;

namespace Consumer;

[ApiController]
[Route("[controller]")]
public class MessageController : ControllerBase
{
    [HttpPost("push")]
    public IActionResult Push(string key, string value)
    {
        Console.WriteLine($"Received message with key: {key}, value: {value}");
        return Ok();
    }
}