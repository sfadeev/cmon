using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CMon.Entities;
using CMon.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Serilog;

namespace CMon
{
	public class UserSecret
	{
		public const string Id = "cmon";
	}

	public class Program
	{
		public static long[] Devices = { 1 , 2 };

		public const short BoardTemp = 0xFF;

		public static IConfigurationRoot Configuration { get; set; }

		public static void Main(string[] args)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
				.AddUserSecrets(UserSecret.Id)
				.AddEnvironmentVariables();

			Configuration = builder.Build();

			Log.Logger = new LoggerConfiguration()
				.ReadFrom.Configuration(Configuration)
				.CreateLogger();

			var state = RunMainThread();

			Log.Logger.Information("Press any key to stop program execution...");

			// Console.Read();

			// http://stackoverflow.com/questions/38549006/docker-container-exits-immediately-even-with-console-readline-in-a-net-core-c?noredirect=1&lq=1
			// http://stackoverflow.com/questions/40506122/control-lifetime-of-net-core-console-application-hosted-in-docker?rq=1
			// https://github.com/aspnet/Hosting/blob/dev/src/Microsoft.AspNetCore.Hosting/WebHostExtensions.cs
			new ManualResetEventSlim(false).Wait();

			Log.CloseAndFlush();

			/*var done = new ManualResetEventSlim(false);
			using (var cts = new CancellationTokenSource())
			{
				Action shutdown = () =>
				{
					if (!cts.IsCancellationRequested)
					{
						Console.WriteLine("Application is shutting down...");
						cts.Cancel();
					}

					done.Wait();
				};

#if NETSTANDARD1_5
                var assemblyLoadContext = AssemblyLoadContext.GetLoadContext(typeof(WebHostExtensions).GetTypeInfo().Assembly);
                assemblyLoadContext.Unloading += context => shutdown();
#endif
				Console.CancelKeyPress += (sender, eventArgs) =>
				{
					shutdown();
					// Don't terminate the process immediately, wait for the Main thread to exit gracefully.
					eventArgs.Cancel = true;
				};

				// host.Run(cts.Token, "Application started. Press Ctrl+C to shut down.");
				done.Set();
			}*/
		}

		private static object RunMainThread()
		{
			var timers = new ConcurrentDictionary<long, Timer>();

			foreach (var deviceId in Devices)
			{
				timers[deviceId] = new Timer(state =>
				{
					try
					{
						PollAsync(deviceId).Wait();
					}
					catch (Exception ex)
					{
						Log.Logger.Error(ex, "Error sending request for device id {deviceId}", deviceId);
					}
				}, deviceId, TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(15));
			}

			return timers;
		}

		public static async Task PollAsync(long deviceId)
		{
			var repository = new DefaultDeviceRepository(Configuration.GetConnectionString("DefaultConnection"));

			var device = repository.GetDevice(deviceId);

			var inputs = repository.GetInputs(deviceId);

			var url = "https://ccu.sh/data.cgx?cmd={\"Command\":\"GetStateAndEvents\"}";

			var json = await Get(device, url);

			if (json != null)
			{
				var jo = JObject.Parse(json);

				// if (jo.SelectToken("Events")?.HasValues == true)
				if (/*device.Id == 1 &&*/ json.Length >= 609)
				{
					Log.Logger.Information(url + "\n" + json);

					// File.WriteAllText("c:\\temp\\ccu\\" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-GetStateAndEvents.json", json);
				}

				var t = GetBoardTemperature(jo);
				repository.SaveToDb(device.Id, BoardTemp, t);

				var message = $"[{device.Id}] - {BoardTemp}:{t:N4}";

				foreach (var input in inputs)
				{
					t = GetInputTemperature(jo, input.InputNo - 1);
					repository.SaveToDb(device.Id, input.InputNo, t);

					message += $" - {input.InputNo}:{t:N4}";
				}

				Log.Logger.Information(message);
			}
		}

		private static async Task<string> Get(DbDevice device, string url)
		{
			using (var client = new HttpClient())
			{
				try
				{
					var auth = $"{device.Username}@{device.Imei}:{device.Password}";
					var authBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(auth));

					client.DefaultRequestHeaders.Add("Authorization", "Basic " + authBase64);

					var response = await client.GetAsync(url);

					// response.EnsureSuccessStatusCode();

					if (response.StatusCode != HttpStatusCode.OK)
					{
						Log.Logger.Error("[{device.Id}] - {response.StatusCode} {response.StatusCode}", device.Id, (int)response.StatusCode, response.StatusCode);
						return null;
					}

					return await response.Content.ReadAsStringAsync();
				}
				catch (HttpRequestException ex)
				{
					Log.Logger.Error(ex, "Get(url) exception");
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
