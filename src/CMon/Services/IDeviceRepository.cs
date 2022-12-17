using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMon.Entities;
using CMon.Models;
using LinqToDB;
using Microsoft.Extensions.Configuration;

namespace CMon.Services
{
	public interface IDeviceRepository
	{
		Task<Device> GetDevice(long deviceId, CancellationToken token);

		Task SaveToDb(long deviceId, short input, decimal value, CancellationToken token);
	}

	public class DefaultDeviceRepository : IDeviceRepository
	{
		private readonly IConfiguration _configuration;

		public DefaultDeviceRepository(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		private DbConnection CreateConnection()
		{
			return new DbConnection(_configuration.GetConnectionString("DefaultConnection"));
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
		
		public async Task SaveToDb(long deviceId, short input, decimal value, CancellationToken token)
		{
			using (var db = CreateConnection())
			{
				await db.InsertAsync(new DbInputValue
				{
					DeviceId = deviceId,
					InputNum = input,
					Value = value,
					CreatedAt = DateTime.UtcNow
				}, token: token);
			}
		}
	}
}