using LinqToDB.Identity;

namespace CMon.Entities
{
	public class DbConnection : IdentityDataConnection<DbUser, DbRole, long>
	{
	}
}
