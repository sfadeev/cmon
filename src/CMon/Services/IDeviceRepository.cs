using System;
using System.Collections.Generic;
using System.Linq;
using CMon.Entities;
using CMon.Models;
using LinqToDB;
using Newtonsoft.Json;

namespace CMon.Services
{
	public interface IDeviceRepository
	{
		IList<DbDevice> GetDevices();

		// DbDevice GetDevice(long deviceId);

		// IList<DbInput> GetInputs(long deviceId);

		void SaveInputValue(long deviceId, short input, decimal value);

		IList<DeviceEvent> LoadEvents(long deviceId, DateTime beginDate, DateTime endDate);

		IEnumerable<DeviceEvent> SaveEvents(long deviceId, IEnumerable<DeviceEvent> events);
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

		/*public DbDevice GetDevice(long deviceId)
		{
			using (var db = _connectionFactory.GetConection())
			{
				return db.GetTable<DbDevice>().SingleOrDefault(x => x.Id == deviceId);
			}
		}*/

		/*public IList<DbInput> GetInputs(long deviceId)
		{
			using (var db = _connectionFactory.GetConection())
			{
				return db.GetTable<DbInput>().Where(x => x.DeviceId == deviceId).ToList();
			}
		}*/

		public void SaveInputValue(long deviceId, short input, decimal value)
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

		public IList<DeviceEvent> LoadEvents(long deviceId, DateTime beginDate, DateTime endDate)
		{
			using (var db = _connectionFactory.GetConection())
			{
				var result = new List<DeviceEvent>();

				var events = db.GetTable<DbEvent>()
					.Where(x => x.DeviceId == deviceId &&
								x.CreatedAt >= beginDate &&
								x.CreatedAt <= endDate);

				foreach (var dbEvent in events)
				{
					result.Add(new DeviceEvent
					{
						Id = dbEvent.Id,
						EventType = dbEvent.EventType,
						ExternalId = dbEvent.ExternalId,
						CreatedAt = dbEvent.CreatedAt,
						CreatedBy = dbEvent.CreatedBy,
						Info = string.IsNullOrEmpty(dbEvent.Info)
							? null : JsonConvert.DeserializeObject<DeviceEventInformation>(dbEvent.Info)
					});
				}

				return result;
			}
		}

		public IEnumerable<DeviceEvent> SaveEvents(long deviceId, IEnumerable<DeviceEvent> events)
		{
			using (var db = _connectionFactory.GetConection())
			{
				var saved = new List<DeviceEvent>();

				foreach (var @event in events)
				{
					var lastDay = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1));

					var dbEvent = db.GetTable<DbEvent>()
						.SingleOrDefault(x => x.DeviceId == deviceId &&
											x.ExternalId == @event.ExternalId &&
											x.CreatedAt > lastDay); // within last day

					if (dbEvent == null)
					{
						dbEvent = new DbEvent
						{
							DeviceId = deviceId,
							CreatedAt = DateTime.UtcNow,
							EventType = @event.EventType,
							ExternalId = @event.ExternalId,
							// Info = JsonConvert.SerializeObject(@event.Info)
						};

						dbEvent.Id = (long) db.InsertWithIdentity(dbEvent);

						// exception on insert - so update instead
						// Npgsql.PostgresException: 42804: column "info" is of type json but expression is of type text

						db.GetTable<DbEvent>()
							.Where(x => x.Id == dbEvent.Id)
							.Set(x => x.Info, JsonConvert.SerializeObject(@event.Info))
							.Update();

						@event.Id = dbEvent.Id;

						saved.Add(@event);
					}
				}

				return saved;
			}
		}
	}
}