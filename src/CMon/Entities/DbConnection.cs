using LinqToDB.DataProvider.PostgreSQL;
using Npgsql;

namespace CMon.Entities
{
	public class DbConnection : LinqToDB.Data.DataConnection
	{
		public DbConnection(string connectionString) : base(
			new PostgreSQLDataProvider("Npgsql", PostgreSQLVersion.v93), new NpgsqlConnection(connectionString))
		{
		}
	}
}