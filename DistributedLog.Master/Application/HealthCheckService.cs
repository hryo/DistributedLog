using DistributedLog.Master.Domain;

namespace DistributedLog.Master.Application
{
    public class HealthCheckService : BackgroundService
    {
        public HealthCheckService()
        {

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
