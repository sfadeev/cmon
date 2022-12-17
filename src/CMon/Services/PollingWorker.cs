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
        private static readonly long[] Devices = { 1 , 2 };
    
        private readonly ILogger<PollingWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly AppSettings _settings;

        public PollingWorker(
            ILogger<PollingWorker> logger, 
            IOptions<AppSettings> settings, 
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
            
                foreach (var deviceId in Devices)
                {
                    try
                    {
                        _logger.LogDebug("Starting polling device id {deviceId}.", deviceId);
                    
                        await poller.Poll(deviceId, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error polling device id {deviceId}.", deviceId);
                    }
                }

                await Task.Delay(delay, cancellationToken);
            }

            _logger.LogInformation("Service is stopping.");
        }
    }
}