namespace DistributedLog.Replica;

public class ReplicaDebugConfiguration
{
    public int StorageDelaySeconds { get; set; } = 0;
    public bool IsHealthy { get; set; } = true;
    public bool ThrowAfterStorageFinished { get; set; } = false;
}