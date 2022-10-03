using DistributedLog.Master.Domain;

namespace DistributedLog.Master.Application;

public class ReplicationMessage
{
    private int _actualCompletions;
    private readonly TaskCompletionSource _completionSource;

    public ReplicationMessage(LogEntry entry, int requiredCompletions)
    {
        _actualCompletions = 0;
        _completionSource = new TaskCompletionSource();
        Entry = entry;
        RequiredCompletions = requiredCompletions;

        // if we don't have to replicate message  
        // it should be marked as already processed immediately after creation
        if (requiredCompletions == 0)
            _completionSource.SetResult();
    }

    public Task Completion => _completionSource.Task;

    public LogEntry Entry { get; }

    public int RequiredCompletions { get; }

    public int ActualCompletions => _actualCompletions;

    public void Processed()
    {
        if (Interlocked.Increment(ref _actualCompletions) >= RequiredCompletions)
            _completionSource.TrySetResult();
    }
}