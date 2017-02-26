using System;
using System.Collections.Generic;
using System.Linq;
using CMon.Entities;
using LinqToDB;
using Microsoft.Extensions.Options;

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
		private readonly IOptions<ConnectionStringOptions> _connectionStringOptions;

		public DefaultDeviceRepository(IOptions<ConnectionStringOptions> connectionStringOptions)
		{
			_connectionStringOptions = connectionStringOptions;
		}

		public DbDevice GetDevice(long deviceId)
		{
			using (var db = new DbConnection(_connectionStringOptions.Value.DefaultConnection))
			{
				return db.GetTable<DbDevice>().SingleOrDefault(x => x.Id == deviceId);
			}
		}

		public IList<DbInput> GetInputs(long deviceId)
		{
			using (var db = new DbConnection(_connectionStringOptions.Value.DefaultConnection))
			{
				return db.GetTable<DbInput>().Where(x => x.DeviceId == deviceId).ToList();
			}
		}

		public void SaveToDb(long deviceId, short input, decimal value)
		{
			using (var db = new DbConnection(_connectionStringOptions.Value.DefaultConnection))
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