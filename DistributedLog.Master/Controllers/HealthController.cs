using DistributedLog.Master.Application;
using DistributedLog.Master.Domain;
using Microsoft.AspNetCore.Mvc;

namespace DistributedLog.Master.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly ReplicationService _storage;

    public HealthController(ReplicationService replicationService)
    {
        _storage = replicationService;
    }

    [HttpGet(Name = "GetRelicasStatuses")]
    public Dictionary<string, string> Get()
    {
        return _storage.GetHealthStatuses().ToDictionary(t => t.Key, t => t.Value.ToString());
    }
}