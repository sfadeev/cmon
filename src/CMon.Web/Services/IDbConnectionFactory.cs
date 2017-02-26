using LinqToDB;

namespace CMon.Services
{
    public interface IDbConnectionFactory
    {
		IDataContext GetContext();
	}

	public class DefaultDbConnectionFactory<TContext> : IDbConnectionFactory
		where TContext : class, IDataContext, new()
	{
		public IDataContext GetContext() => new TContext();
	}
}
