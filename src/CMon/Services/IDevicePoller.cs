using System;
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
        private readonly CcuSettings _settings;

        public DefaultDevicePoller(
            ILogger<DefaultDevicePoller> logger,
            IOptions<CcuSettings> settings,
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
                ce.SlidingExpiration = TimeSpan.FromMinutes(_settings.CacheMinutes);

                return await _gateway.GetProfilesInitial(auth, cancellationToken);
            });
            
            var controlPoll = await _gateway.GetControlPoll(auth, cancellationToken);
            var stateAndEvents = await _gateway.GetStateAndEvents(auth, cancellationToken);

            if (stateAndEvents.Status.Code == StatusCode.Ok)
            {
                _metricFactory.CreateGauge($"ccu_modem_status", "").Set(controlPoll.ControlPoll.ModemStatus);
                _metricFactory.CreateGauge($"ccu_signal_dbm", "").Set(controlPoll.ControlPoll.Signal.dBm);
                _metricFactory.CreateGauge($"ccu_signal_percent", "").Set(controlPoll.ControlPoll.Signal.Percent);
                _metricFactory.CreateGauge($"ccu_signal_strength", "").Set(controlPoll.ControlPoll.Signal.Strength);
                
                _metricFactory.CreateGauge($"ccu_balance", "Баланс").Set(stateAndEvents.Balance);
                _metricFactory.CreateGauge($"ccu_temp", "Температура основной платы").Set(stateAndEvents.Temp);
                _metricFactory.CreateGauge($"ccu_case", "Датчик вскрытия корпуса").Set(stateAndEvents.Case);
                _metricFactory.CreateGauge($"ccu_battery_charge", "Уровень заряда батареи").Set(stateAndEvents.Battery.Charge ?? 0);
                _metricFactory.CreateGauge($"ccu_battery_state", "Состояние батареи").Set((int)stateAndEvents.Battery.State);
                _metricFactory.CreateGauge($"ccu_power", "Основное питание").Set(stateAndEvents.Power);
                
                for (var i = 0; i < stateAndEvents.Inputs.Length; i++)
                {
                    var input = stateAndEvents.Inputs[i];
                    
                    var no = i + 1;
                    var name = initial.InputsSchema[i];
                    var type = initial.ProfilesInitial.Inputs[i].InputType;
                    
                    ConvertInputDiscrete(type, input.Voltage, out decimal voltage, out var temp);

                    if (_logger.IsEnabled(LogLevel.Debug))
                    {
                        _logger.LogDebug("[{no}] {name} ({type}) - active:{active}, discrete:{discrete}, voltage:{voltage:N2}, temp:{temp:N2}",
                            no, name, type, input.Active, input.Voltage, voltage, temp);    
                    }
                    
                    _metricFactory.CreateGauge($"ccu_in{no}_active", name + " active").Set(input.Active);
                    _metricFactory.CreateGauge($"ccu_in{no}_discrete", name + " discrete").Set(input.Voltage);
                    _metricFactory.CreateGauge($"ccu_in{no}_voltage", name + " voltage").Set((double)voltage);
                    
                    if (temp.HasValue)
                    {
                        _metricFactory.CreateGauge($"ccu_in{no}_temp", name + " temp").Set((double)temp);
                    }
                }

                for (var i = 0; i < stateAndEvents.Outputs.Length; i++)
                {
                    var output = stateAndEvents.Outputs[i];
                    
                    var outputNo = i + 1;
                    var name = initial.OutputsSchema[i];
                    
                    _metricFactory.CreateGauge($"ccu_out{outputNo}_active", name + " active").Set(output);
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
                /*temp = -3.03641M * (decimal)Math.Pow((double)voltage, 3) 
                    + 25.5916M * (decimal)Math.Pow((double)voltage, 2) 
                    - 87.9556M * voltage + 120.641M;*/

                temp = -40.3289M * (decimal)Math.Log(0.28738 * (double)voltage);
            }
        }
    }
}