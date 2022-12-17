namespace CMon.Entities
{
	public class DbConnection : LinqToDB.Data.DataConnection
	{
		public DbConnection(string connectionString) : base(LinqToDB.ProviderName.PostgreSQL, connectionString)
		{
		}
	}
}