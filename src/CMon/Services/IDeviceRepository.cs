using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMon.Entities;
using CMon.Models;
using LinqToDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Prometheus.Client;

namespace CMon.Services
{
	public interface IDeviceRepository
	{
		Task<Device> GetDevice(long deviceId, CancellationToken token);

		Task SaveToDb(Device device, short input, string name, decimal value, CancellationToken token);
	}

	public class DefaultDeviceRepository : IDeviceRepository
	{
		private readonly ILogger<DefaultDeviceRepository> _logger;
		private readonly IConfiguration _configuration;
		private readonly IMetricFactory _metricFactory;

		public DefaultDeviceRepository(ILogger<DefaultDeviceRepository> logger,
			IConfiguration configuration, IMetricFactory metricFactory)
		{
			_logger = logger;
			_configuration = configuration;
			_metricFactory = metricFactory;
		}

		private DbConnection CreateConnection()
		{
			var connectionString = _configuration.GetConnectionString("DefaultConnection");
			
			// _logger.LogInformation("Using connection string {cs}", connectionString);
			
			return new DbConnection(connectionString);
		}
		
		public async Task<Device> GetDevice(long deviceId, CancellationToken token)
		{
			using (var db = CreateConnection())
			{
				var device = await db.GetTable<DbDevice>()
					.Where(x => x.Id == deviceId)
					.Select(x => new Device
					{
						Id = x.Id,
						Imei = x.Imei,
						Username = x.Username,
						Password = x.Password,
					})
					.SingleOrDefaultAsync(token);

				if (device != null)
				{
					device.Inputs = await db.GetTable<DbInput>()
						.Where(x => x.DeviceId == deviceId).ToListAsync(token);
				}

				return device;
			}
		}
		
		public async Task SaveToDb(Device device, short input, string name, decimal value, CancellationToken token)
		{
			using (var db = CreateConnection())
			{
				await db.InsertAsync(new DbInputValue
				{
					DeviceId = device.Id,
					InputNum = input,
					Value = value,
					CreatedAt = DateTime.UtcNow
				}, token: token);
			}
		}
	}
}