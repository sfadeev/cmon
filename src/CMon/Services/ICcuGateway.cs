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

		Task<InputsInitialResult> GetInputsInitial(Auth auth);

		Task<InputsPollResult> GetInputsPoll(Auth auth, int inputNum);

		Task<InputsInputNumResult> GetInputsInputNum(Auth auth, int inputNum);

		Task<ProfilesInitialResult> GetProfilesInitial(Auth auth);

		Task<ProfilesProfNumResult> GetProfilesProfNum(Auth auth, int profNum);

		Task<SystemInitialResult> GetSystemInitial(Auth auth);

		Task<SystemPollResult> GetSystemPoll(Auth auth);

		Task<DeviceInfo> GetDeviceInfo(Auth auth);

		Task<StateAndEventsResult> GetStateAndEvents(Auth auth);
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
			return await Get<IndexInitialResult>(auth, GetCmdUrl(new { DataType = "IndexInitial" }));
		}

		public async Task<ControlInitialResult> GetControlInitial(Auth auth)
		{
			return await Get<ControlInitialResult>(auth, GetCmdUrl(new { DataType = "ControlInitial" }));
		}

		public async Task<ControlPollResult> GetControlPoll(Auth auth)
		{
			return await Get<ControlPollResult>(auth, GetCmdUrl(new { DataType = "ControlPoll" }));
		}

		public async Task<InputsInitialResult> GetInputsInitial(Auth auth)
		{
			return await Get<InputsInitialResult>(auth, GetCmdUrl(new { DataType = "InputsInitial" }));
		}

		public async Task<InputsPollResult> GetInputsPoll(Auth auth, int inputNum)
		{
			return await Get<InputsPollResult>(auth, GetCmdUrl(new { DataType = "InputsPoll", InputNum = inputNum }));
		}

		public async Task<InputsInputNumResult> GetInputsInputNum(Auth auth, int inputNum)
		{
			return await Get<InputsInputNumResult>(auth, GetCmdUrl(new { DataType = "InputsInputNum", InputNum = inputNum }));
		}

		public async Task<ProfilesInitialResult> GetProfilesInitial(Auth auth)
		{
			return await Get<ProfilesInitialResult>(auth, GetCmdUrl(new { DataType = "ProfilesInitial" }));
		}

		public async Task<ProfilesProfNumResult> GetProfilesProfNum(Auth auth, int profNum)
		{
			return await Get<ProfilesProfNumResult>(auth, GetCmdUrl(new { DataType = "ProfilesProfNum", ProfNum = profNum }));
		}

		public async Task<SystemInitialResult> GetSystemInitial(Auth auth)
		{
			return await Get<SystemInitialResult>(auth, GetCmdUrl(new { DataType = "SystemInitial" }));
		}

		public async Task<SystemPollResult> GetSystemPoll(Auth auth)
		{
			return await Get<SystemPollResult>(auth, GetCmdUrl(new { DataType = "SystemPoll" }));
		}

		public async Task<DeviceInfo> GetDeviceInfo(Auth auth)
		{
			return await Get<DeviceInfo>(auth, GetCmdUrl(new { Command = "GetDeviceInfo" }));
		}

		public async Task<StateAndEventsResult> GetStateAndEvents(Auth auth)
		{
			return await Get<StateAndEventsResult>(auth, GetCmdUrl(new { Command = "GetStateAndEvents" }));
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

					_logger.LogDebug("Requesting url {0}", url);

					var response = await client.GetAsync(url);

					TResult result;

					_logger.LogDebug("Request to url {0} completed with code {1}", url, response.StatusCode);

					if (response.StatusCode == HttpStatusCode.OK)
					{
						var content = await response.Content.ReadAsStringAsync();

						try
						{
							result = JsonConvert.DeserializeObject<TResult>(content);

							if (result.Status == null) result.Status = new Status { Code = StatusCode.Ok };

						}
						catch (Exception ex)
						{
							_logger.LogError(0, ex, "Failed to deserialize content {0}", content);

							throw;
						}
					}
					else
					{
						result = new TResult();
					}

					result.HttpStatusCode = response.StatusCode;

					return result;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(0, ex, "Failed to load {0} for {1} from {2}", typeof(TResult).Name, auth?.DebuggerDisplay, url);
			}

			return default(TResult);
		}

		private static string GetCmdUrl(object cmd)
		{
			return $"{BaseUrl}?cmd=" + JsonConvert.SerializeObject(cmd);
		}
	}
}
