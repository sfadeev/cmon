using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CMon.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace CMon.Services
{
    public interface IDevicePoller
    {
        Task Poll(long deviceId, CancellationToken cancellationToken);
    }

    public class DefaultDevicePoller : IDevicePoller
    {
        private const short BoardTemp = 0xFF;
        
        private readonly ILogger<DefaultDevicePoller> _logger;
        private readonly AppSettings _settings;
        private readonly IDeviceRepository _repository;

        public DefaultDevicePoller(
            ILogger<DefaultDevicePoller> logger,
            IOptions<AppSettings> settings,
            IDeviceRepository repository)
        {
            _logger = logger;
            _settings = settings.Value;
            _repository = repository;
        }
        
        public async Task Poll(long deviceId, CancellationToken cancellationToken)
        {
            var device = await _repository.GetDevice(deviceId, cancellationToken);

            var url = _settings.BaseUrl + "/data.cgx?cmd={\"Command\":\"GetStateAndEvents\"}";

            var json = await Get(device, url);

            if (json != null)
            {
                var jo = JObject.Parse(json);
                
                if (json.Length >= 609)
                {
                    _logger.LogDebug("{url}\n{json}", url, json);
                }

                var t = GetBoardTemperature(jo);
                
                await _repository.SaveToDb(device, BoardTemp, null, t, cancellationToken);

                var message = $"[{device.Id}] {DateTime.Now:s} - {BoardTemp}:{t:N4}";

                foreach (var input in device.Inputs)
                {
                    if (input.Type == InputType.Rtd02 || input.Type == InputType.Rtd03 || input.Type == InputType.Rtd04)
                    {
                        var discrete = GetInputDiscrete(jo, input.InputNo - 1);

                        t = GetInputTemperature(input.Type, discrete);

                        await _repository.SaveToDb(device, input.InputNo, input.Name, t, cancellationToken);

                        message += $" - {input.InputNo}:{t:N4}";
                    }
                }

                _logger.LogInformation(message);
            }
        }

        private async Task<string> Get(DbDevice device, string url)
        {
            using (var client = new HttpClient(new HttpClientHandler
                   {
                       ServerCertificateCustomValidationCallback =
                           HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                   }))
            {
                try
                {
                    var auth = $"{device.Username}@{device.Imei}:{device.Password}";
                    var authBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(auth));

                    client.DefaultRequestHeaders.Add("Authorization", "Basic " + authBase64);

                    var response = await client.GetAsync(url);

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        _logger.LogError("Error response from device [{deviceId}] - {statusCode} ({status})",
                            device.Id, (int) response.StatusCode, response.StatusCode);

                        return null;
                    }

                    return await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "Get(url) exception.");
                }
            }

            return null;
        }
        
        private static decimal GetInputTemperature(InputType inputType, long discrete)
        {
            const decimal maxRangeVal = 4095;
            
            var voltage = discrete * 10M / maxRangeVal;

            decimal temp;

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

                // temp = -40.3289M * (decimal)Math.Log(0.28738 * (double)voltage);
            }
            else
            {
                throw new InvalidOperationException($"Input type {inputType} is not supported.");
            }

            return temp;
        }
        
        private static long GetInputDiscrete(JObject jo, int input)
        {
            var discrete = jo.SelectToken($"Inputs[{input}].Voltage")!.Value<long>();
            
            return discrete;
        }
        
        private static decimal GetInputTemperature(JObject jo, int input)
        {
            var discrete = GetInputDiscrete(jo, input);
            
            var voltage = discrete * 10M / 4095;
            var temp = (voltage / 5M - 0.5M) / 0.01M;

            return temp;
        }

        private static decimal GetBoardTemperature(JObject jo)
        {
            var temp = jo.SelectToken("Temp")!.Value<long>();

            return temp;
        }
    }
}