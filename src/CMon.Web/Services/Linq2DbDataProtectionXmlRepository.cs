using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using CMon.Services;
using CMon.Web.Entities;
using LinqToDB;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.Logging;

namespace CMon.Web.Services
{
    public class Linq2DbDataProtectionXmlRepository : IXmlRepository
	{
		private readonly ILogger<Linq2DbDataProtectionXmlRepository> _logger;
		private readonly IDbConnectionFactory _connectionFactory;

		public Linq2DbDataProtectionXmlRepository(
			ILogger<Linq2DbDataProtectionXmlRepository> logger, IDbConnectionFactory connectionFactory)
		{
			_logger = logger;
			_connectionFactory = connectionFactory;
		}

		public IReadOnlyCollection<XElement> GetAllElements()
		{
			var elements = _connectionFactory.GetContext()
				.GetTable<DbDataProtectionKey>()
				.Select(key => XElement.Parse(key.Data))
				.ToList();

			_logger.LogDebug("[{hashCode}] GetAllElements({count})", GetHashCode(), elements.Count);

			return new ReadOnlyCollection<XElement>(elements);
		}

		public void StoreElement(XElement element, string friendlyName)
		{
			_logger.LogDebug("[{hashCode}] StoreElement({friendlyName})", GetHashCode(), friendlyName);

			_connectionFactory.GetContext().Insert(new DbDataProtectionKey
			{
				Id = Guid.NewGuid().ToString(),
				Data = element.ToString(),
				CreatedAt = DateTime.UtcNow
			});
		}
	}
}
