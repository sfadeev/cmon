using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CMon.Models.Ccu;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CMon.Services
{
	public interface ICcuGateway
	{
		Task<IndexInitialResult> GetIndexInitial(Auth auth, CancellationToken cancellationToken = default);

		Task<ControlInitialResult> GetControlInitial(Auth auth, CancellationToken cancellationToken = default);

		Task<ControlPollResult> GetControlPoll(Auth auth, CancellationToken cancellationToken = default);

		Task<InputsInitialResult> GetInputsInitial(Auth auth, CancellationToken cancellationToken = default);

		Task<InputsPollResult> GetInputsPoll(Auth auth, int inputNum, CancellationToken cancellationToken = default);

		Task<InputsInputNumResult> GetInputsInputNum(Auth auth, int inputNum, CancellationToken cancellationToken = default);

		Task<ProfilesInitialResult> GetProfilesInitial(Auth auth, CancellationToken cancellationToken = default);

		Task<ProfilesProfNumResult> GetProfilesProfNum(Auth auth, int profNum, CancellationToken cancellationToken = default);

		Task<SystemInitialResult> GetSystemInitial(Auth auth, CancellationToken cancellationToken = default);

		Task<SystemPollResult> GetSystemPoll(Auth auth, CancellationToken cancellationToken = default);

		Task<DeviceInfo> GetDeviceInfo(Auth auth, CancellationToken cancellationToken = default);

		Task<StateAndEventsResult> GetStateAndEvents(Auth auth, CancellationToken cancellationToken = default);

		Task<AckEventsResult> AckEvents(Auth auth, long[] ids, CancellationToken cancellationToken = default);
	}

	public class CcuGateway : ICcuGateway
	{
		private readonly ILogger<CcuGateway> _logger;
		private readonly CcuSettings _settings;
		private readonly IHttpClientFactory _httpClientFactory;

		public CcuGateway(ILogger<CcuGateway> logger, IOptions<CcuSettings> settings, IHttpClientFactory httpClientFactory)
		{
			_logger = logger;
			_settings = settings.Value;
			_httpClientFactory = httpClientFactory;
		}

		public async Task<IndexInitialResult> GetIndexInitial(Auth auth, CancellationToken cancellationToken = default)
		{
			return await Get<IndexInitialResult>(auth, GetCmdUrl(new { DataType = "IndexInitial" }), cancellationToken);
		}

		public async Task<ControlInitialResult> GetControlInitial(Auth auth, CancellationToken cancellationToken = default)
		{
			return await Get<ControlInitialResult>(auth, GetCmdUrl(new { DataType = "ControlInitial" }), cancellationToken);
		}

		public async Task<ControlPollResult> GetControlPoll(Auth auth, CancellationToken cancellationToken = default)
		{
			return await Get<ControlPollResult>(auth, GetCmdUrl(new { DataType = "ControlPoll" }), cancellationToken);
		}

		public async Task<InputsInitialResult> GetInputsInitial(Auth auth, CancellationToken cancellationToken = default)
		{
			return await Get<InputsInitialResult>(auth, GetCmdUrl(new { DataType = "InputsInitial" }), cancellationToken);
		}

		public async Task<InputsPollResult> GetInputsPoll(Auth auth, int inputNum, CancellationToken cancellationToken = default)
		{
			return await Get<InputsPollResult>(auth, GetCmdUrl(new { DataType = "InputsPoll", InputNum = inputNum }), cancellationToken);
		}

		public async Task<InputsInputNumResult> GetInputsInputNum(Auth auth, int inputNum, CancellationToken cancellationToken = default)
		{
			return await Get<InputsInputNumResult>(auth, GetCmdUrl(new { DataType = "InputsInputNum", InputNum = inputNum }), cancellationToken);
		}

		public async Task<ProfilesInitialResult> GetProfilesInitial(Auth auth, CancellationToken cancellationToken = default)
		{
			return await Get<ProfilesInitialResult>(auth, GetCmdUrl(new { DataType = "ProfilesInitial" }), cancellationToken);
		}

		public async Task<ProfilesProfNumResult> GetProfilesProfNum(Auth auth, int profNum, CancellationToken cancellationToken = default)
		{
			return await Get<ProfilesProfNumResult>(auth, GetCmdUrl(new { DataType = "ProfilesProfNum", ProfNum = profNum }), cancellationToken);
		}

		public async Task<SystemInitialResult> GetSystemInitial(Auth auth, CancellationToken cancellationToken = default)
		{
			return await Get<SystemInitialResult>(auth, GetCmdUrl(new { DataType = "SystemInitial" }), cancellationToken);
		}

		public async Task<SystemPollResult> GetSystemPoll(Auth auth, CancellationToken cancellationToken = default)
		{
			return await Get<SystemPollResult>(auth, GetCmdUrl(new { DataType = "SystemPoll" }), cancellationToken);
		}

		public async Task<DeviceInfo> GetDeviceInfo(Auth auth, CancellationToken cancellationToken = default)
		{
			return await Get<DeviceInfo>(auth, GetCmdUrl(new { Command = "GetDeviceInfo" }), cancellationToken);
		}

		public async Task<StateAndEventsResult> GetStateAndEvents(Auth auth, CancellationToken cancellationToken = default)
		{
			return await Get<StateAndEventsResult>(auth, GetCmdUrl(new { Command = "GetStateAndEvents" }), cancellationToken);
		}

		public async Task<AckEventsResult> AckEvents(Auth auth, long[] ids, CancellationToken cancellationToken = default)
		{
			return await Get<AckEventsResult>(auth, GetCmdUrl(new { Command = "AckEvents", IDs = ids }), cancellationToken);
		}

		private async Task<TResult> Get<TResult>(Auth auth, string url, CancellationToken cancellationToken) where TResult : CommandResult, new()
		{
			if (auth.Imei == null) throw new ArgumentNullException(nameof(auth.Imei));
			if (auth.Username == null) throw new ArgumentNullException(nameof(auth.Username));
			if (auth.Password == null) throw new ArgumentNullException(nameof(auth.Password));

			try
			{
				using (var client = _httpClientFactory.CreateClient())
				{
					_logger.LogDebug("{auth} - requesting {url}", auth.DebuggerDisplay,url);

					var authStr = $"{auth.Username}@{auth.Imei}:{auth.Password}";
					var authBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(authStr));

					client.BaseAddress = new Uri(_settings.BaseUrl);
					client.DefaultRequestHeaders.Add("Authorization", "Basic " + authBase64);

					var stopwatch = new Stopwatch();
					stopwatch.Start();
					
					var response = await client.GetAsync(url, cancellationToken);

					TResult result;

					if (response.StatusCode == HttpStatusCode.OK)
					{
						_logger.LogDebug("{auth} - request {type} completed with code {statusCode}, elapsed {elapsed}",
							auth.DebuggerDisplay, typeof(TResult).Name, response.StatusCode, stopwatch.Elapsed);

						var content = await response.Content.ReadAsStringAsync(cancellationToken);

						try
						{
							result = JsonConvert.DeserializeObject<TResult>(content);

							result.Status ??= new Status { Code = StatusCode.Ok };
						}
						catch (Exception ex)
						{
							_logger.LogError(ex, "{auth} - failed to deserialize {type} content: \n {content}",
								auth.DebuggerDisplay, typeof(TResult).Name, content);

							throw;
						}
					}
					else
					{
						_logger.LogInformation(
							"{auth} - request {type} failed with code {statusCode}, elapsed {elapsed}",
							auth.DebuggerDisplay, typeof(TResult).Name, response.StatusCode, stopwatch.Elapsed);

						result = new TResult();
					}

					result.HttpStatusCode = response.StatusCode;

					return result;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "{auth} - failed to load {type} from {url}", auth.DebuggerDisplay, typeof(TResult).Name, url);
			}

			return default;
		}

		private string GetCmdUrl(object cmd)
		{
			return "?cmd=" + JsonConvert.SerializeObject(cmd);
		}
	}
}
