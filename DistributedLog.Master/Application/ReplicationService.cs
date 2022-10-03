using System.Diagnostics;
using DistributedLog.Master.Domain;

namespace DistributedLog.Master.Application
{
    public class ReplicationService
    {
        private readonly ILogger<ReplicationService> _logger;
        private readonly ReplicationTarget[] _targets;

        public ReplicationService(ReplicasFactory replicasFactory, ILogger<ReplicationService> logger)
        {
            _logger = logger;
            _targets = replicasFactory.GetTargets().ToArray();
        }

        public async Task Replicate(LogEntry entry, int replicationFactor)
        {
            var sw = Stopwatch.StartNew();
            var requiredReplicasToSucceed = replicationFactor - 1;
            var replicationMessage = new ReplicationMessage(entry, requiredReplicasToSucceed);
            foreach (var replicationTarget in _targets)
            {
                await replicationTarget.Append(replicationMessage);
            }

            await replicationMessage.Completion;
            sw.Stop();
            _logger.LogInformation("{entry} was replicated. {required}, {actual}. Elapsed: {elapsed}",
                entry, replicationMessage.RequiredCompletions, replicationMessage.ActualCompletions, sw.Elapsed);
        }

        public Dictionary<string, HealthStatus> GetHealthStatuses() =>
            _targets.ToDictionary(t => t.Name, t => t.Status);
    }
}
