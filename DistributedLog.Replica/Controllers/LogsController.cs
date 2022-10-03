using DistributedLog.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace DistributedLog.Replica.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogsController : ControllerBase
    {
        private readonly LogStorage _logStorage;
        private readonly ReplicaDebugConfiguration _debugConfiguration;
        private readonly ILogger<LogsController> _logger;

        public LogsController(LogStorage logStorage, ReplicaDebugConfiguration debugConfiguration, ILogger<LogsController> logger)
        {
            _logStorage = logStorage;
            _debugConfiguration = debugConfiguration;
            _logger = logger;
        }

        [HttpPost(Name = "AppendMessage")]
        public async Task<IActionResult> Append(LogEntryDto entry)
        {
            _logger.LogInformation("Received request {entry}", entry);

            await Task.Delay(TimeSpan.FromSeconds(_debugConfiguration.StorageDelaySeconds));

            _logStorage.Append(entry.Id, entry.Message);

            _logger.LogInformation("{entry} was stored", entry);

            if (_debugConfiguration.ThrowAfterStorageFinished)
                throw new Exception("Modeling transient errors and deduplication");

            return Ok();
        }

        [HttpGet(Name = "ReadMessages")]
        public string[] ReadMessages()
        {
            return _logStorage.ReadAll().ToArray();
        }
    }
}