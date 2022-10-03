using DistributedLog.Master.Domain;

namespace DistributedLog.Master.Application;

public class ReplicatedStorage
{
    private readonly LogStorage _storage;
    private readonly ReplicationService _replicationService;

    public ReplicatedStorage(LogStorage storage, ReplicationService replicationService)
    {
        _storage = storage;
        _replicationService = replicationService;
    }

    public async Task Append(string message, int replicationFactor)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(message));
        if (replicationFactor <= 0) 
            throw new ArgumentOutOfRangeException(nameof(replicationFactor));

        var entry = _storage.Append(message);
        await _replicationService.Replicate(entry, replicationFactor);
    }

    public IEnumerable<LogEntry> ReadAll()
    {
        return _storage.ReadAll();
    }
}