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

		public TResult Dispatch<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>
		{
			var queryHandler = _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();

			return queryHandler.Retrieve(query);
		}
	}
}