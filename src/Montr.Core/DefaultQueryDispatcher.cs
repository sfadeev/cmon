using System;
using Microsoft.Extensions.DependencyInjection;

namespace Montr.Core
{
	public class DefaultQueryDispatcher : IQueryDispatcher
	{
		private readonly IServiceProvider _serviceProvider;

		public DefaultQueryDispatcher(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public TQueryResult Dispatch<TQuery, TQueryResult>(TQuery query) where TQuery : IQuery<TQueryResult>
		{
			return _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TQueryResult>>().Retrieve(query);
		}
	}
}