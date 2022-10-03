namespace DistributedLog.Master.Application
{
    public class ReplicationConfig
    {
        public string[] Urls { get; set; }
        public double HealthChecksIntervalSeconds { get; set; }
    }
}
