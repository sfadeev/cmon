using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CMon.Models.Ccu;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CMon.Services
{
	public interface ICcuGateway
	{
		Task<IndexInitialResult> GetIndexInitial(Auth auth);

		Task<ControlInitialResult> GetControlInitial(Auth auth);

		Task<ControlPollResult> GetControlPoll(Auth auth);

		Task<DeviceInfo> GetDeviceInfo(Auth auth);

		Task<StateAndEvents> GetStateAndEvents(Auth auth);
	}

	public class CcuGateway : ICcuGateway
	{
		private const string BaseUrl = "https://ccu.sh/data.cgx";

		private readonly ILogger<CcuGateway> _logger;

		public CcuGateway(ILogger<CcuGateway> logger)
		{
			_logger = logger;
		}

		public async Task<IndexInitialResult> GetIndexInitial(Auth auth)
		{
			return await Get<IndexInitialResult>(auth, GetDataTypeUrl("IndexInitial"));
		}

		public async Task<ControlPollResult> GetControlPoll(Auth auth)
		{
			return await Get<ControlPollResult>(auth, GetDataTypeUrl("ControlPoll"));
		}

		public async Task<ControlInitialResult> GetControlInitial(Auth auth)
		{
			return await Get<ControlInitialResult>(auth, GetDataTypeUrl("ControlInitial"));
		}

		public async Task<DeviceInfo> GetDeviceInfo(Auth auth)
		{
			return await Get<DeviceInfo>(auth, GetCommandUrl("GetDeviceInfo"));
		}

		public async Task<StateAndEvents> GetStateAndEvents(Auth auth)
		{
			return await Get<StateAndEvents>(auth, GetCommandUrl("GetStateAndEvents"));
		}

		private async Task<TResult> Get<TResult>(Auth auth, string url) where TResult : CommandResult, new()
		{
			try
			{
				using (var client = new HttpClient())
				{
					var authStr = $"{auth.Username}@{auth.Imei}:{auth.Password}";
					var authBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(authStr));

					client.DefaultRequestHeaders.Add("Authorization", "Basic " + authBase64);

					var response = await client.GetAsync(url);

					TResult result;

					if (response.StatusCode == HttpStatusCode.OK)
					{
						var content = await response.Content.ReadAsStringAsync();

						result = JsonConvert.DeserializeObject<TResult>(content);
					}
					else
					{
						result = new TResult();
					}

					result.HttpStatusCode = response.StatusCode;

					return result;
				}
			}
			catch (HttpRequestException ex)
			{
				_logger.LogError(0, ex, "Failed to load {0} for {1} from {2}", typeof(TResult).Name, auth?.DebuggerDisplay, url);
			}

			return default(TResult);
		}

		private static string GetDataTypeUrl(string dataType)
		{
			return $"{BaseUrl}?cmd={{\"DataType\":\"{dataType}\"}}";
		}

		private static string GetCommandUrl(string command)
		{
			return $"{BaseUrl}?cmd={{\"Command\":\"{command}\"}}";
		}
	}
}
