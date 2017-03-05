namespace Montr.Core
{
	public interface ICommand<out TResult>
	{
	}

	public interface ICommandHandler<in TCommand, out TResult> where TCommand : ICommand<TResult>
	{
		TResult Execute(TCommand command);
	}

	public interface ICommandDispatcher
	{
		TResult Dispatch<TCommand, TResult>(TCommand command) where TCommand : ICommand<TResult>;
	}
}