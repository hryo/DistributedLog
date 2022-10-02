namespace DistributedLog.Master.Domain
{
    public record LogEntry
    {
        public Guid Id { get; init; }
        public string Message { get; init; }

        public LogEntry(string message)
        {
            ArgumentNullException.ThrowIfNull(message);

            Id = Guid.NewGuid();
            Message = message;
        }
    }
}
