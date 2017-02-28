using System;
using System.Collections.Generic;
using System.Linq;
using CMon.Entities;
using LinqToDB;

namespace CMon.Services
{
	public interface IDeviceRepository
	{
		IList<DbDevice> GetDevices();

		DbDevice GetDevice(long deviceId);

		IList<DbInput> GetInputs(long deviceId);

		void SaveToDb(long deviceId, short input, decimal value);
	}

	public class DefaultDeviceRepository : IDeviceRepository
	{
		private readonly IDbConnectionFactory _connectionFactory;

		public DefaultDeviceRepository(IDbConnectionFactory connectionFactory)
		{
			_connectionFactory = connectionFactory;
		}

		public IList<DbDevice> GetDevices()
		{
			using (var db = _connectionFactory.GetConection())
			{
				return db.GetTable<DbDevice>().ToList();
			}
		}

		public DbDevice GetDevice(long deviceId)
		{
			using (var db = _connectionFactory.GetConection())
			{
				return db.GetTable<DbDevice>().SingleOrDefault(x => x.Id == deviceId);
			}
		}

		public IList<DbInput> GetInputs(long deviceId)
		{
			using (var db = _connectionFactory.GetConection())
			{
				return db.GetTable<DbInput>().Where(x => x.DeviceId == deviceId).ToList();
			}
		}

		public void SaveToDb(long deviceId, short input, decimal value)
		{
			using (var db = _connectionFactory.GetConection())
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
	}
}