using System.Net;
using DistributedLog.Master.Domain;

namespace DistributedLog.Master.Application
{
    public class ReplicaApiClient
    {
        private const string LogUrl = "logs";
        private const string PingUrl = "ping";
        private readonly HttpClient _client;
        private readonly ILogger<ReplicaApiClient> _logger;

        public ReplicaApiClient(HttpClient client, ILogger<ReplicaApiClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        public string Url => _client.BaseAddress.ToString();

        public async Task AppendMessage(LogEntry message)
        {
            try
            {
                var response = await _client.PostAsJsonAsync(LogUrl, message);
                response.EnsureSuccessStatusCode();
                _logger.LogInformation("Entry {entry} was published to {url}", message, _client.BaseAddress);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to append {message} to {url}", message, Url);
                throw new FailedToAppendMessageException(message, e);
            }
        }

        public async Task<HealthStatus> Ping()
        {
            try
            {
                var response = await _client.GetAsync(PingUrl);
                response.EnsureSuccessStatusCode();
                return HealthStatus.Alive;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to ping {url}.", Url);
                return HealthStatus.Dead;
            }
        }
    }
}
