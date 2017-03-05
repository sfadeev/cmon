namespace Montr.Core
{
	public interface ICommand
	{
	}

	public interface ICommandHandler<in TCommand> where TCommand : ICommand
	{
		void Execute(TCommand command);
	}

	public interface ICommandDispatcher
	{
		void Dispatch<TParameter>(TParameter command) where TParameter : ICommand;
	}

}