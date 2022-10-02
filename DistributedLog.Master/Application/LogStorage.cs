using DistributedLog.Master.Domain;
using System.Collections.Concurrent;

namespace DistributedLog.Master.Application
{
    public interface ILogStorage
    {
        ValueTask Append(LogEntry entry);
        IEnumerable<LogEntry> ReadAll();
    }

    public class LogStorage : ILogStorage
    {
        private readonly ILogger _logger;
        private readonly ConcurrentQueue<LogEntry> _messages;

        public LogStorage(ILogger<LogStorage> logger)
        {
            _messages = new ConcurrentQueue<LogEntry>();
            _logger = logger;
        }

        public ValueTask Append(LogEntry entry)
        {
            _messages.Enqueue(entry);
            _logger.LogInformation("Entry appended {entry}", entry);
            return ValueTask.CompletedTask;
        }

        public IEnumerable<LogEntry> ReadAll()
        {
            return _messages;
        }
    }

    public class ReplicatedStorage : ILogStorage
    {
        private readonly ILogStorage _storage;
        private readonly ReplicationProtocol _protocol;

        public ReplicatedStorage(ILogStorage storage, ReplicationProtocol protocol)
        {
            _storage = storage;
            _protocol = protocol;
        }

        public async ValueTask Append(LogEntry entry)
        {
            await _storage.Append(entry);
            await _protocol.Replicate(entry);
        }

        public IEnumerable<LogEntry> ReadAll()
        {
            return _storage.ReadAll();
        }
    }
}
