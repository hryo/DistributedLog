using DistributedLog.Master.Domain;

namespace DistributedLog.Master.Application
{
    public class FailedToAppendMessageException : Exception
    {
        public FailedToAppendMessageException(LogEntry entry, Exception exception)
            : base($"Failed to append message Id: {entry.Id}, Message: {entry.Message}", exception)
        {
        }
    }
}
