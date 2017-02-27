using LinqToDB;
using LinqToDB.Data;

namespace CMon.Services
{
    public interface IDbConnectionFactory
    {
		IDataContext GetContext();

	    DataConnection GetConection();
    }

	public class DefaultDbConnectionFactory<TContext, TConnection> : IDbConnectionFactory
		where TContext : class, IDataContext, new()
		where TConnection : DataConnection, new()
	{
		public DataConnection GetConection() => new TConnection();

		public IDataContext GetContext() => new TContext();
	}
}
