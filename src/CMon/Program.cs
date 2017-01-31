using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CMon.Entities;
using CMon.Services;
using LinqToDB;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace CMon
{
	public class Program
	{
		public static long[] Devices = { 1, 2 };

		public const short BoardTemp = 0xFF;

		public static IConfigurationRoot Configuration { get; set; }

		public static void Main(string[] args)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.AddUserSecrets()
				.AddEnvironmentVariables();

			Configuration = builder.Build();

			var state = RunMainThread();

			var color = Console.ForegroundColor;

			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.WriteLine("Press any key to stop program execution...");
			Console.ForegroundColor = color;

			Console.ReadLine();
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
						SendRequest(deviceId).Wait();
					}
					catch (Exception e)
					{
						Console.WriteLine($"Error sending request for device id {deviceId}\n" + e);
					}
				}, deviceId, TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(15));
			}

			return timers;
		}

		public static void SaveToDb(long deviceId, short input, decimal value)
		{
			using (var db = new DbConnection(Configuration.GetConnectionString("DefaultConnection")))
			{
				db.Insert(new DbInputValue
				{
					DeviceId = deviceId,
					InputNum = input,
					Value = value,
					CreatedAt = DateTime.UtcNow
				});
			}
		}

		public static async Task SendRequest(long deviceId)
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
					Console.WriteLine(url + "\n" + json);
					Console.WriteLine();

					File.WriteAllText("c:\\temp\\ccu\\" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-GetStateAndEvents.json", json);
				}

				var t = GetBoardTemperature(jo);
				SaveToDb(device.Id, BoardTemp, t);

				var message = $"[{device.Id}] - {DateTime.Now:s}  |  [{BoardTemp}] : {t:N4}";

				foreach (var input in inputs)
				{
					t = GetInputTemperature(jo, input.InputNo - 1);
					SaveToDb(device.Id, input.InputNo, t);

					message += $"  |  [{input.InputNo}] : {t:N4}";
				}

				Console.WriteLine(message);
			}
		}

		private static async Task<string> Get(DbDevice device, string url)
		{
			using (var client = new HttpClient())
			{
				try
				{
					var auth = $"{device.Username}@{device.Imei}:{device.Password}";

					client.DefaultRequestHeaders.Add(
						"Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(auth)));

					var response = await client.GetAsync(url);
					response.EnsureSuccessStatusCode();

					return await response.Content.ReadAsStringAsync();
				}
				catch (HttpRequestException ex)
				{
					Console.WriteLine("Get(url) exception.\n" + ex);
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
