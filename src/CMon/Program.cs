using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CMon.Entities;
using LinqToDB;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace CMon
{
	public class Program
	{
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

			RunMainThread();
		}

		private static void RunMainThread()
		{
			int defaultTimeout = 15000,
				errorTimeout = 60000;

			var timeout = 0;

			while (true)
			{
				try
				{
					Console.WriteLine($"Sleeping {timeout} ms");

					Thread.Sleep(timeout);

					SendRequest().Wait();

					Console.WriteLine();

					timeout = defaultTimeout;
				}
				catch (Exception e)
				{
					Console.WriteLine("Main exception.\n" + e);

					timeout = errorTimeout;
				}
			}
		}

		public static DbDevice GetDevice(long deviceId)
		{
			using (var db = new DbConnection(Configuration.GetConnectionString("DefaultConnection")))
			{
				return db.GetTable<DbDevice>().SingleOrDefault(x => x.Id == deviceId);
			}
		}

		public static void SaveToDb(long deviceId, short input, decimal value)
		{
			Console.WriteLine($"[{input}] : {value:N2}");

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

		public static async Task SendRequest()
		{
			var device = GetDevice(0);

			// await Get("https://ccu.sh/data.cgx?cmd={\"DataType\":\"ControlPoll\"}");
			/*
			 * 
			 * var level = dataPoll["SignalLevel"];
			 * var levelPercent = Math.round(level * 100 / 31);
			 * 
			 * dataPoll["ModemStatus"]
			 * 
			 * <option data-loc="si_modem_0"
>Инициализация модема...</option>
<option data-loc="si_modem_1"
>Сервисный режим, обновление прошивки GSM модуля</option>
<option data-loc="si_modem_2"
>Основное питание отсутствует, регистрация в GSM сети невозможна</option>
<option data-loc="si_modem_3"
>SIM-карта не установлена</option>
<option data-loc="si_modem_4"
>SIM-карта неисправна или отсутствует</option>
<option data-loc="si_modem_5"
>Неверный PIN-код</option>
<option data-loc="si_modem_6"
>SIM-карта заблокирована, требуется PUK-код</option>
<option data-loc="si_modem_7"
>PIN-код принят</option>
<option data-loc="si_modem_8"
>Зарегистрирован в домашней сети</option>
<option data-loc="si_modem_9"
>Регистрация в процессе...</option>
<option data-loc="si_modem_10"
>Зарегистрирован в роуминге</option>
<option data-loc="si_modem_11"
>Неизвестное состояние GSM модуля</option>			 
			 */

			// await Get("https://ccu.sh/data.cgx?cmd={\"Command\":\"GetDeviceInfo\"}");

			var url = "https://ccu.sh/data.cgx?cmd={\"Command\":\"GetStateAndEvents\"}";

			var json = await Get(device, url);

			if (json != null)
			{
				var jo = JObject.Parse(json);

				// if (jo.SelectToken("Events")?.HasValues == true)
				if (json.Length >= 600)
				{
					Console.WriteLine(url + "\n" + json);
					Console.WriteLine();

					File.WriteAllText("c:\\temp\\ccu\\" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-GetStateAndEvents.json", json);
				}

				var t = GetBoardTemperature(jo);
				SaveToDb(device.Id, BoardTemp, t);

				var t0 = GetInputTemperature(jo, 0);
				SaveToDb(device.Id, 0, t0);

				var t1 = GetInputTemperature(jo, 1);
				SaveToDb(device.Id, 1, t1);
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
