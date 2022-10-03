using System.Threading.Channels;
using DistributedLog.Master.Domain;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace DistributedLog.Master.Application;

public class ReplicationTarget : IAsyncDisposable
{
    private readonly ReplicaApiClient _apiClient;
    private readonly Channel<ReplicationMessage> _channel;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly Task _replicationTask;
    private readonly Task _health;
    private readonly CancellationTokenSource _completion;

    public ReplicationTarget(ReplicaApiClient apiClient, TimeSpan healChecksInterval, ILogger<ReplicationTarget> logger)
    {
        _apiClient = apiClient;
        _completion = new CancellationTokenSource();
        _channel = Channel.CreateUnbounded<ReplicationMessage>(new UnboundedChannelOptions()
        {
            SingleReader = true,
            AllowSynchronousContinuations = true
        });

        _retryPolicy = Policy.Handle<Exception>()
            .WaitAndRetryForeverAsync(
                i => TimeSpan.FromSeconds(i % 10),
                (exception, i, delay) => logger.LogWarning($"Retry {i} for target {apiClient.Url}"));

        _health = StartHealthChecks(apiClient, healChecksInterval, _completion.Token);

        _replicationTask = _channel.Reader
            .ReadAllAsync()
            .ForEachAwaitAsync(entry => SendMessageToReplica(apiClient, entry), _completion.Token);
    }

    public HealthStatus Status { get; private set; } = HealthStatus.Unknown;
    public string Name => _apiClient.Url;

    public ValueTask Append(ReplicationMessage message)
    {
        return _channel.Writer.WriteAsync(message);
    }

    public async ValueTask DisposeAsync()
    {
        _completion.Cancel();
        await _health;
        await _replicationTask;
    }

    private Task SendMessageToReplica(ReplicaApiClient apiClient, ReplicationMessage message)
    {
        return _retryPolicy.ExecuteAsync(async () =>
        {
            if (Status == HealthStatus.Dead)
                throw new ReplicaNotAvailableException(apiClient.Url);

            await apiClient.AppendMessage(message.Entry);
            message.Processed();
            Status = HealthStatus.Alive;
        });
    }

    private async Task StartHealthChecks(ReplicaApiClient apiClient, TimeSpan interval, CancellationToken cancellation)
    {
        while (!cancellation.IsCancellationRequested)
        {
            Status = await apiClient.Ping();
            await Task.Delay(interval, cancellation);
        }
    }
}