using System;
using System.Diagnostics;
using System.Net;
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

		Task<AckEventsResult> AckEvents(Auth auth, long[] ids);
	}

	public class CcuGateway : ICcuGateway
	{
		private const string BaseUrl = "https://ccu.su/data.cgx";

		private readonly ILogger<CcuGateway> _logger;
		private readonly IHttpClientFactory _httpClientFactory;

		public CcuGateway(ILogger<CcuGateway> logger, IHttpClientFactory httpClientFactory)
		{
			_logger = logger;
			_httpClientFactory = httpClientFactory;
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

		public async Task<AckEventsResult> AckEvents(Auth auth, long[] ids)
		{
			return await Get<AckEventsResult>(auth, GetCmdUrl(new { Command = "AckEvents", IDs = ids }));
		}

		// private readonly ConcurrentDictionary<string, AsyncLock> _locks = new ConcurrentDictionary<string, AsyncLock>();

		private async Task<TResult> Get<TResult>(Auth auth, string url) where TResult : CommandResult, new()
		{
			// var awaiter = _locks.GetOrAdd(auth.Imei, x => new AsyncLock());

			// using (await awaiter.LockAsync())
			{
				try
				{
					using (var client = _httpClientFactory.CreateClient())
					{
						var authStr = $"{auth.Username}@{auth.Imei}:{auth.Password}";
						var authBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(authStr));

						client.AddHeader("Authorization", "Basic " + authBase64);

						_logger.LogDebug("{0} - requesting {1}", auth.DebuggerDisplay, typeof(TResult).Name);

						var stopwatch = new Stopwatch();
						stopwatch.Start();

						var response = await client.GetAsync(url);

						TResult result;

						var test = await response.ReadContentAsync();
						
						if (response.StatusCode == HttpStatusCode.OK)
						{
							_logger.LogDebug("{0} - request {1} completed with code {2}, elapsed {3}", auth.DebuggerDisplay, typeof(TResult).Name, response.StatusCode, stopwatch.Elapsed);

							var content = await response.ReadContentAsync();

							try
							{
								result = JsonConvert.DeserializeObject<TResult>(content);

								if (result.Status == null) result.Status = new Status { Code = StatusCode.Ok };
							}
							catch (Exception ex)
							{
								_logger.LogError(0, ex, "{0} - failed to deserialize {1} content: \n {2}", auth.DebuggerDisplay, typeof(TResult).Name, content);

								throw;
							}
						}
						else
						{
							_logger.LogInformation("{0} - request {1} failed with code {2}, elapsed {3}", auth.DebuggerDisplay, typeof(TResult).Name, response.StatusCode, stopwatch.Elapsed);
							
							result = new TResult();
						}

						result.HttpStatusCode = response.StatusCode;

						return result;
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(0, ex, "{0} - failed to load {1} for {2} from {3}", auth.DebuggerDisplay, typeof(TResult).Name, auth.DebuggerDisplay, url);
				}

				return default(TResult);
			}
		}

		private static string GetCmdUrl(object cmd)
		{
			return $"{BaseUrl}?cmd=" + JsonConvert.SerializeObject(cmd);
		}
	}
}
