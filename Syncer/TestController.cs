using Microsoft.AspNetCore.Mvc;
using Syncer.RoutingAlgorithm;

namespace Syncer;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("brokers")]
    public string Brokers()
    {
        return string.Join(' ', ENVIRONMENT.ALL_BROKERS);
    }
    
    [HttpGet("routers")]
    public string Routers()
    {
        return string.Join(' ', ENVIRONMENT.ALL_ROUTERS);
    }
}