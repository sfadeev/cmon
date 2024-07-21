using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CMon.Services
{
    public class PollingWorker : BackgroundService
    {
        private readonly ILogger<PollingWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly CcuSettings _settings;

        public PollingWorker(
            ILogger<PollingWorker> logger, 
            IOptions<CcuSettings> settings, 
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _settings = settings.Value;
        }
    
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var delay = TimeSpan.FromSeconds(_settings.PollSeconds);
        
            _logger.LogInformation("Service is starting, poll interval {delay}.", delay);
        
            while (cancellationToken.IsCancellationRequested == false)
            {
                var poller = _serviceProvider.GetRequiredService<IDevicePoller>();

                try
                {
                    await poller.Poll(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error polling");
                }

                await Task.Delay(delay, cancellationToken);
            }

            _logger.LogInformation("Service is stopping.");
        }
    }
}