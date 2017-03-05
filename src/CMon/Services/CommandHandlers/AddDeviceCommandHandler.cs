using CMon.Commands;
using CMon.Entities;
using LinqToDB;
using Montr.Core;

namespace CMon.Services.CommandHandlers
{
	public class AddDeviceCommandHandler : ICommandHandler<AddDevice, long>
	{
		private readonly IDbConnectionFactory _connectionFactory;

		public AddDeviceCommandHandler(IDbConnectionFactory connectionFactory)
		{
			_connectionFactory = connectionFactory;
		}

		public long Execute(AddDevice command)
		{
			using (var db = _connectionFactory.GetConection())
			{
				var dbDevice = new DbDevice
				{
					Imei = command.Imei,
					Username = command.Username,
					Password = command.Password
				};

				db.Insert(dbDevice);

				return dbDevice.Id;
			}
		}
	}
}
