using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Montr.Core
{
	public class DefaultCommandDispatcher : ICommandDispatcher
	{
		private readonly IServiceProvider _serviceProvider;

		public DefaultCommandDispatcher(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public Task<TResult> Dispatch<TParameter, TResult>(TParameter command) where TParameter : ICommand<TResult>
		{
			var commandHandler = _serviceProvider.GetRequiredService<ICommandHandler<TParameter, TResult>>();

			return commandHandler.Execute(command);
		}
	}
}