namespace Montr.Core
{
	public interface IQuery<out TQueryResult>
	{
	}

	public interface IQueryHandler<in TQuery, out TQueryResult> where TQuery : IQuery<TQueryResult>
	{
		TQueryResult Retrieve(TQuery query);
	}

	public interface IQueryDispatcher
	{
		TQueryResult Dispatch<TQuery, TQueryResult>(TQuery query) where TQuery : IQuery<TQueryResult>;
	}
}