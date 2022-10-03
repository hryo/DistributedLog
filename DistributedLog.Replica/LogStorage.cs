using System.Collections.Concurrent;

namespace DistributedLog.Replica;

public class LogStorage
{
    private readonly ConcurrentQueue<Guid> _ids;
    private readonly ConcurrentDictionary<Guid, string> _messages;

    public LogStorage()
    {
        _ids = new ConcurrentQueue<Guid>();
        _messages = new ConcurrentDictionary<Guid, string>();
    }

    public void Append(Guid id, string message)
    {
        if (_messages.TryAdd(id, message))
        {
            _ids.Enqueue(id);
        }
    }

    public IEnumerable<string> ReadAll()
    {
        foreach (var id in _ids)
        {
            yield return _messages[id];
        }
    }
}