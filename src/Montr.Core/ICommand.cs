using System.Threading.Tasks;

namespace Montr.Core
{
	public interface ICommand<out TResult>
	{
	}

	public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
	{
		Task<TResult> Execute(TCommand command);
	}

	public interface ICommandDispatcher
	{
		Task<TResult> Dispatch<TCommand, TResult>(TCommand command) where TCommand : ICommand<TResult>;
	}
}