using System.Collections.Concurrent;

namespace DistributedLog.Master.Domain
{
    public class LogStorage
    {
        private long _order;
        private readonly ILogger _logger;
        private readonly ConcurrentQueue<LogEntry> _messages;

        public LogStorage(ILogger<LogStorage> logger)
        {
            _messages = new ConcurrentQueue<LogEntry>();
            _logger = logger;
        }

        public LogEntry Append(string message)
        {
            var entry = new LogEntry(message);
            _messages.Enqueue(entry);
            _logger.LogInformation("Entry appended {entry}", entry);
            return entry;
        }

        public IEnumerable<LogEntry> ReadAll()
        {
            return _messages;
        }
    }
}
