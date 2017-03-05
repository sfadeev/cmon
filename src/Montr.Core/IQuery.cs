namespace Montr.Core
{
	public interface IQuery<out TResult>
	{
	}

	public interface IQueryHandler<in TQuery, out TResult> where TQuery : IQuery<TResult>
	{
		TResult Retrieve(TQuery query);
	}

	public interface IQueryDispatcher
	{
		TResult Dispatch<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>;
	}
}