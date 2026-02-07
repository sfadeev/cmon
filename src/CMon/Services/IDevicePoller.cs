using System;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CMon.Models.Ccu;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prometheus.Client;

namespace CMon.Services
{
    public interface IDevicePoller
    {
        Task Poll(CancellationToken cancellationToken);
    }

    public class DefaultDevicePoller : IDevicePoller
    {
        private readonly ILogger<DefaultDevicePoller> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly ICcuGateway _gateway;
        private readonly IMetricFactory _metricFactory;
        private readonly AppOptions _settings;

        public DefaultDevicePoller(
            ILogger<DefaultDevicePoller> logger,
            IOptions<AppOptions> settings,
            IMemoryCache memoryCache,
            ICcuGateway gateway,
            IMetricFactory metricFactory)
        {
            _logger = logger;
            _memoryCache = memoryCache;
            _gateway = gateway;
            _metricFactory = metricFactory;
            _settings = settings.Value;
        }

        public async Task Poll(CancellationToken cancellationToken)
        {
            var auth = new Auth
            {
                Imei = _settings.Imei,
                Username = _settings.Username,
                Password = _settings.Password 
            };

            var initial = await _memoryCache.GetOrCreateAsync(nameof(ProfilesInitial), async ce =>
            {
                ce.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_settings.CacheMinutes);

                return await _gateway.GetProfilesInitial(auth, cancellationToken);
            });
            
            var controlPoll = await _gateway.GetControlPoll(auth, cancellationToken);
            
            if (controlPoll?.Status?.Code == StatusCode.Ok)
            {
                _metricFactory.CreateGauge("ccu_modem_status", string.Empty).Set(controlPoll.ControlPoll.ModemStatus);
                _metricFactory.CreateGauge("ccu_signal_dbm", string.Empty).Set(controlPoll.ControlPoll.Signal.dBm);
                _metricFactory.CreateGauge("ccu_signal_percent", string.Empty).Set(controlPoll.ControlPoll.Signal.Percent);
                _metricFactory.CreateGauge("ccu_signal_strength", string.Empty).Set(controlPoll.ControlPoll.Signal.Strength);
            }
            
            var stateAndEvents = await _gateway.GetStateAndEvents(auth, cancellationToken);
            
            if (stateAndEvents?.Status?.Code == StatusCode.Ok)
            {
                if (double.TryParse(stateAndEvents.Power, CultureInfo.InvariantCulture, out var power))
                {
                    _metricFactory.CreateGauge("ccu_power_active", string.Empty).Set(1);
                    _metricFactory.CreateGauge("ccu_power_voltage", string.Empty).Set(power);
                }
                else
                {
                    _metricFactory.CreateGauge("ccu_power_active", string.Empty).Set(0);
                }
                
                if (double.TryParse(stateAndEvents.Balance, CultureInfo.InvariantCulture, out var balance))
                {
                    _metricFactory.CreateGauge("ccu_balance", string.Empty).Set(balance);     
                }
                
                _metricFactory.CreateGauge("ccu_temp", string.Empty).Set(stateAndEvents.Temp);
                _metricFactory.CreateGauge("ccu_case", string.Empty).Set(stateAndEvents.Case);
                _metricFactory.CreateGauge("ccu_battery_charge", string.Empty).Set(stateAndEvents.Battery.Charge ?? 0);
                _metricFactory.CreateGauge("ccu_battery_state", string.Empty).Set((int)stateAndEvents.Battery.State);
                
                var sb = _logger.IsEnabled(LogLevel.Debug) ? new StringBuilder() : null;
                
                for (var i = 0; i < stateAndEvents.Inputs.Length; i++)
                {
                    var input = stateAndEvents.Inputs[i];
                    
                    var num = i + 1;
                    var name = initial?.InputsSchema?[i];
                    var type = initial?.ProfilesInitial?.Inputs[i].InputType ?? InputType.Rtd03;
                    
                    ConvertInputDiscrete(type, input.Voltage, out decimal voltage, out var temp);

                    if (_logger.IsEnabled(LogLevel.Debug) && sb != null)
                    {
                        sb.Append($"[{num}] ({type}) {name} - discrete:{input.Voltage}, voltage:{voltage:N2}");

                        if (temp.HasValue) sb.Append($", temp:{temp:N2}");
                        
                        sb.AppendLine();    
                    }
                    
                    _metricFactory.CreateGauge($"ccu_in{num}_active", name).Set(input.Active);
                    _metricFactory.CreateGauge($"ccu_in{num}_discrete", name).Set(input.Voltage);
                    _metricFactory.CreateGauge($"ccu_in{num}_voltage", name).Set((double)voltage);
                    
                    if (temp.HasValue)
                    {
                        _metricFactory.CreateGauge($"ccu_in{num}_temp", name).Set((double)temp);
                    }
                }

                if (_logger.IsEnabled(LogLevel.Debug) && sb != null) _logger.LogDebug(sb.ToString());

                for (var i = 0; i < stateAndEvents.Outputs.Length; i++)
                {
                    var output = stateAndEvents.Outputs[i];
                    
                    var num = i + 1;
                    var name = initial?.OutputsSchema?[i];
                    
                    _metricFactory.CreateGauge($"ccu_out{num}_active", name).Set(output);
                }
            }
        }
        
        private static void ConvertInputDiscrete(InputType inputType, long discrete, out decimal voltage, out decimal? temp)
        {
            const decimal maxRangeVal = 4095;
            
            voltage = discrete * 10M / maxRangeVal;
            temp = null;

            if (inputType == InputType.Rtd02)
            {
                temp = (voltage / 3M - 0.5M) / 0.01M;
            }
            else if (inputType == InputType.Rtd03)
            {
                temp = (voltage / 5M - 0.5M) / 0.01M;
            }
            else if (inputType == InputType.Rtd04)
            {
                temp = -3.03641M * (decimal)Math.Pow((double)voltage, 3) 
                    + 25.5916M * (decimal)Math.Pow((double)voltage, 2) 
                    - 87.9556M * voltage + 120.641M;

                // wrong convert for output temp
                // temp = -40.3289M * (decimal)Math.Log(0.28738 * (double)voltage);
            }
        }
    }
}