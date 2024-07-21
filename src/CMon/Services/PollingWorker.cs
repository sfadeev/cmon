using System;
using System.Diagnostics;
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
            var pollDelay = TimeSpan.FromSeconds(_settings.PollSeconds);
        
            _logger.LogInformation("Service starting, poll interval {delay}.", pollDelay);

            while (cancellationToken.IsCancellationRequested == false)
            {
                var sw = new Stopwatch();
                sw.Start();
                
                try
                {
                    var poller = _serviceProvider.GetRequiredService<IDevicePoller>();

                    await poller.Poll(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error polling");
                }

                var sleepDelay = pollDelay - sw.Elapsed;

                if (sleepDelay > TimeSpan.Zero)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug("Service sleeping for {delay}.", sleepDelay);
                
                    await Task.Delay(sleepDelay, cancellationToken);
                }
            }

            _logger.LogInformation("Service stopping.");
        }
    }
}