using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Montr.Core
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddCqrs(this IServiceCollection services)
		{
			services.AddScoped<IQueryDispatcher, DefaultQueryDispatcher>();
			services.AddScoped<ICommandDispatcher, DefaultCommandDispatcher>();

			return services;
		}

		public static IServiceCollection AddQueryHandlers(this IServiceCollection services, params Assembly[] assemblies)
		{
			// todo: discover services from assemblies

			return services;
		}

		public static IServiceCollection AddCommandHandlers(this IServiceCollection services, params Assembly[] assemblies)
		{
			// todo: discover services from assemblies

			return services;
		}
	}
}
