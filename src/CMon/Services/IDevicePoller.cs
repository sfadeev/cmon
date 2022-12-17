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
                
                await _repository.SaveToDb(device.Id, BoardTemp, t, cancellationToken);

                var message = $"[{device.Id}] {DateTime.Now:s} - {BoardTemp}:{t:N4}";

                foreach (var input in device.Inputs)
                {
                    t = GetInputTemperature(jo, input.InputNo - 1);
                    
                    await _repository.SaveToDb(device.Id, input.InputNo, t, cancellationToken);

                    message += $" - {input.InputNo}:{t:N4}";
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

        private static decimal GetInputTemperature(JObject jo, int input)
        {
            var discrete = jo.SelectToken($"Inputs[{input}].Voltage").Value<long>();
            var voltage = discrete * 10M / 4095;
            var temp = (voltage / 5M - 0.5M) / 0.01M;

            return temp;
        }

        private static decimal GetBoardTemperature(JObject jo)
        {
            var temp = jo.SelectToken("Temp").Value<long>();

            return temp;
        }
    }
}