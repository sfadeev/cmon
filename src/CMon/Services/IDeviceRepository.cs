using System;
using System.Collections.Generic;
using System.Linq;
using CMon.Entities;
using LinqToDB;

namespace CMon.Services
{
	public interface IDeviceRepository
	{
		DbDevice GetDevice(long deviceId);

		IList<DbInput> GetInputs(long deviceId);

		void SaveToDb(long deviceId, short input, decimal value);
	}

	public class DefaultDeviceRepository : IDeviceRepository
	{
		private readonly string _connectionString;

		public DefaultDeviceRepository(string connectionString)
		{
			_connectionString = connectionString;
		}

		public DbDevice GetDevice(long deviceId)
		{
			using (var db = new DbConnection(_connectionString))
			{
				return db.GetTable<DbDevice>().SingleOrDefault(x => x.Id == deviceId);
			}
		}

		public IList<DbInput> GetInputs(long deviceId)
		{
			using (var db = new DbConnection(_connectionString))
			{
				return db.GetTable<DbInput>().Where(x => x.DeviceId == deviceId).ToList();
			}
		}

		public void SaveToDb(long deviceId, short input, decimal value)
		{
			using (var db = new DbConnection(_connectionString))
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