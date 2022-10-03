using Microsoft.AspNetCore.Mvc;

namespace DistributedLog.Replica.Controllers;

[ApiController]
[Route("[controller]")]
public class SettingsController : Controller
{
    private readonly ReplicaDebugConfiguration _debugConfiguration;

    public SettingsController(ReplicaDebugConfiguration debugConfiguration)
    {
        _debugConfiguration = debugConfiguration;
    }

    [HttpPut]
    public IActionResult SetStorageDelay(ReplicaDebugConfiguration debugConfiguration)
    {
        _debugConfiguration.IsHealthy = debugConfiguration.IsHealthy;
        _debugConfiguration.StorageDelaySeconds = debugConfiguration.StorageDelaySeconds;
        _debugConfiguration.ThrowAfterStorageFinished = debugConfiguration.ThrowAfterStorageFinished;
        return Ok();
    }
}