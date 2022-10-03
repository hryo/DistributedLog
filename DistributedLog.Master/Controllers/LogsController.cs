using DistributedLog.Master.Application;
using DistributedLog.Master.Application.Requests;
using DistributedLog.Master.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DistributedLog.Master.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogsController : ControllerBase
    {
        private readonly ReplicatedStorage _storage;

        public LogsController(ReplicatedStorage storage)
        {
            _storage = storage;
        }

        [HttpGet(Name = "GetLogEntries")]
        [ProducesResponseType(typeof(string[]), 200)]
        public async Task<IActionResult> Get()
        {
            var entries = _storage.ReadAll();
            return Ok(entries.Select(e => e.Message).ToArray());
        }

        [HttpPost(Name = "AppendMessage")]
        public async Task<IActionResult> Post(AppendMessageRequest request)
        {
            await _storage.Append(request.Message, request.ReplicationFactor);
            return Ok();
        }
    }
}