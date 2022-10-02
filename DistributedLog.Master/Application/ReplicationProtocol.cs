using DistributedLog.Master.Domain;

namespace DistributedLog.Master.Application
{
    public class ReplicationProtocol
    {
        private readonly ILogger<ReplicationProtocol> _logger;
        private SecondariesFactory _secondariesFactory;

        public ReplicationProtocol(SecondariesFactory secondariesFactory, ILogger<ReplicationProtocol> logger)
        {
            _logger = logger;
            _secondariesFactory = secondariesFactory;
        }

        public async Task Replicate(LogEntry entry)
        {
            var secondaries = _secondariesFactory.GetSecondaries();
            var appendTasks = secondaries.Select(s => s.AppendMessage(entry)).ToArray();
            await Task.WhenAll(appendTasks);
        }
    }

    public class SecondariesFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SecondariesFactory(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public IEnumerable<SecodaryClient> GetSecondaries()
        {
            return _serviceProvider.GetServices<SecodaryClient>();
        }
    }
}
