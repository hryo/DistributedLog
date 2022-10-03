using Microsoft.Extensions.Options;

namespace DistributedLog.Master.Application;

public class ReplicasFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ReplicationConfig _config;

    public ReplicasFactory(
        IOptions<ReplicationConfig> optConfig,
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _config = optConfig.Value;
    }

    public IEnumerable<ReplicationTarget> GetTargets()
    {
        var healthChecksInterval = TimeSpan.FromSeconds(_config.HealthChecksIntervalSeconds);
        foreach (var url in _config.Urls)
        {
            var httpClient = _serviceProvider.GetService<HttpClient>();
            httpClient.BaseAddress = new Uri(url);
            var logger = _serviceProvider.GetRequiredService<ILogger<ReplicaApiClient>>();
            var client = new ReplicaApiClient(httpClient, logger);
            var targetLogger = _serviceProvider.GetService<ILogger<ReplicationTarget>>();

            yield return new ReplicationTarget(client, healthChecksInterval, targetLogger);
        }
    }
}