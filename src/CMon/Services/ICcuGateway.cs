using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CMon.Services
{
	public interface ICcuGateway
	{
		Task<ControlInitial> GetControlInitial(CcuAuth ccuAuth);

		Task<DeviceInfo> GetDeviceInfo(CcuAuth ccuAuth);

		Task<StateAndEvents> GetStateAndEvents(CcuAuth auth);
	}

	public class CcuGateway : ICcuGateway
	{
		private readonly ILogger<CcuGateway> _logger;

		public CcuGateway(ILogger<CcuGateway> logger)
		{
			_logger = logger;
		}

		public async Task<ControlInitial> GetControlInitial(CcuAuth ccuAuth)
		{
			try
			{
				var url = GetDataTypeUrl("ControlInitial");

				using (var client = CreateHttpClient(ccuAuth))
				{
					var response = await client.GetAsync(url);

					// _logger.LogError("[{device.Id}] - {response.StatusCode} {response.StatusCode}", device.Id, (int)response.StatusCode, response.StatusCode);

					if (response.StatusCode == HttpStatusCode.OK)
					{
						var content = await response.Content.ReadAsStringAsync();

						// {"Status":{"Code":-9,"Description":"insufficient rights"}}

						var result = JsonConvert.DeserializeObject<ControlInitial>(content);

						return result;
					}
				}
			}
			catch (HttpRequestException ex)
			{
				_logger.LogError(0, ex, ex.Message);
			}

			return null;
		}

		public async Task<DeviceInfo> GetDeviceInfo(CcuAuth ccuAuth)
		{
			try
			{
				var url = GetCommandUrl("GetDeviceInfo");

				using (var client = CreateHttpClient(ccuAuth))
				{
					var response = await client.GetAsync(url);

					// _logger.LogError("[{device.Id}] - {response.StatusCode} {response.StatusCode}", device.Id, (int)response.StatusCode, response.StatusCode);

					if (response.StatusCode == HttpStatusCode.OK)
					{
						var content = await response.Content.ReadAsStringAsync();

						var result = JsonConvert.DeserializeObject<DeviceInfo>(content);

						return result;
					}
				}
			}
			catch (HttpRequestException ex)
			{
				_logger.LogError(0, ex, ex.Message);
			}

			return null;
		}

		public async Task<StateAndEvents> GetStateAndEvents(CcuAuth ccuAuth)
		{
			try
			{
				var url = GetCommandUrl("GetStateAndEvents");

				using (var client = CreateHttpClient(ccuAuth))
				{
					var response = await client.GetAsync(url);

					// _logger.LogError("[{device.Id}] - {response.StatusCode} {response.StatusCode}", device.Id, (int)response.StatusCode, response.StatusCode);

					if (response.StatusCode == HttpStatusCode.OK)
					{
						var content = await response.Content.ReadAsStringAsync();

						var jo = JObject.Parse(content);

						var result = jo.ToObject<StateAndEvents>();

						return result;
					}
				}
			}
			catch (HttpRequestException ex)
			{
				_logger.LogError(0, ex, ex.Message);
			}

			return null;
		}

		private static string GetDataTypeUrl(string dataType)
		{
			return $"https://ccu.sh/data.cgx?cmd={{\"DataType\":\"{dataType}\"}}";
		}

		private static string GetCommandUrl(string command)
		{
			return $"https://ccu.sh/data.cgx?cmd={{\"Command\":\"{command}\"}}";
		}

		private static HttpClient CreateHttpClient(CcuAuth ccuAuth)
		{
			var client = new HttpClient();

			var auth = $"{ccuAuth.Username}@{ccuAuth.Imei}:{ccuAuth.Password}";
			var authBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(auth));

			client.DefaultRequestHeaders.Add("Authorization", "Basic " + authBase64);

			return client;
		}
	}

	public class CcuAuth
	{
		public string Imei { get; set; }

		public string Username { get; set; }

		public string Password { get; set; }
	}

	public class ControlInitial
	{
		public CurrentUser CurrentUser { get; set; }
	}

	public class CurrentUser
	{
		public string Name { get; set; }
	}

	public class StateAndEvents
	{
		public Input[] Inputs { get; set; }

		public byte[] Outputs { get; set; }

		public string[] Partitions { get; set; }

		public Battery Battery { get; set; }

		public byte Case { get; set; }

		public float Power { get; set; }

		public long Temp { get; set; }

		public float Balance { get; set; }
	}

	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class Input
	{
		public byte Active { get; set; }

		public int Voltage { get; set; }

		private string DebuggerDisplay => $"Active: {Active}, Voltage: {Voltage}";
	}

	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class Battery
	{
		public string State { get; set; }

		public int Charge { get; set; }

		private string DebuggerDisplay => $"State: {State}, Charge: {Charge}";
	}

	public class DeviceInfo
	{
		public string DeviceType { get; set; }

		public string DeviceMod { get; set; }

		public string HwVer { get; set; }

		public string FwVer { get; set; }

		public string BootVer { get; set; }

		public string FwBuildDate { get; set; }

		public string CountryCode { get; set; }

		public string Serial { get; set; }

		public string Imei { get; set; }

		public int InputsCount { get; set; }

		public int PartitionsCount { get; set; }

		public string ExtBoard { get; set; }

		public string uGuardVerCode { get; set; }
	}
}
