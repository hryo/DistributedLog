using DistributedLog.Master.Domain;

namespace DistributedLog.Master.Application
{
    public class SecodaryClient
    {
        private const string LogUrl = "logs";
        private readonly HttpClient _client;
        private readonly ILogger<SecodaryClient> _logger;

        public SecodaryClient(HttpClient client, ILogger<SecodaryClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task AppendMessage(LogEntry message)
        {
            try
            {
                _logger.LogInformation("Entry {entry} was published to {url}", message, _client.BaseAddress);
                var response = await _client.PostAsJsonAsync(LogUrl, message);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                throw new FailedToAppendMessageException(message, e);
            }
        }
    }
}
