using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace DistributedLog.Replica.Controllers;

[ApiController]
[Route("[controller]")]
public class PingController : Controller
{
    private readonly ReplicaDebugConfiguration _debugConfiguration;

    public PingController(ReplicaDebugConfiguration debugConfiguration)
    {
        _debugConfiguration = debugConfiguration;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return _debugConfiguration.IsHealthy 
            ? Ok() 
            : StatusCode((int)HttpStatusCode.InternalServerError);
    }
}