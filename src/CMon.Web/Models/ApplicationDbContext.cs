using CMon.Web.Entities;
using LinqToDB.Identity;

namespace CMon.Web.Models
{
	public class ApplicationDataConnection : IdentityDataConnection<DbUser, IdentityRole<long>, long>
	{
		/*public ApplicationDataConnection()
		{
		}*/

		/*public ApplicationDataConnection(string connectionString) : base(new PostgreSQLDataProvider(PostgreSQLVersion.v93), connectionString)
		{
		}*/
	}
}
