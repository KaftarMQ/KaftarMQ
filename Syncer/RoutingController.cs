using Microsoft.AspNetCore.Mvc;

namespace Syncer;

[ApiController]
[Route("[controller]")]
public class RoutingController : ControllerBase
{
    [HttpPost("key")]
    public IActionResult AddKey(string key)
    {
        Console.WriteLine($"New key: {key} is requested to be Added");
        return Ok();
    }
}