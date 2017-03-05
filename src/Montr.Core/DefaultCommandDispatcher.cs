using System;
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

		public void Dispatch<TParameter>(TParameter command) where TParameter : ICommand
		{
			_serviceProvider.GetRequiredService<ICommandHandler<TParameter>>().Execute(command);
		}
	}
}