using LinqToDB.Identity;

namespace CMon.Web.Entities
{
	public class IdentityDbConnection : IdentityDataConnection<DbUser, DbRole, long>
	{
	}
}
