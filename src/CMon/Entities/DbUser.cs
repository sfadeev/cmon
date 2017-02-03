using System;
using LinqToDB.Mapping;

namespace CMon.Entities
{
	[Table(Schema = "public", Name = "users")]
	public class DbUser
	{
		[Column("id"), PrimaryKey, Identity]
		public long Id { get; set; }

		[Column("user_name"), NotNull]
		public string UserName { get; set; }

		[Column("first_name")]
		public string FirstName { get; set; }

		[Column("last_name")]
		public string LastName { get; set; }

		[Column("email")]
		public string Email { get; set; }

		[Column("email_confirmed"), NotNull]
		public bool EmailConfirmed { get; set; }

		[Column("phone_number")]
		public string PhoneNumber { get; set; }

		[Column("phone_number_confirmed"), NotNull]
		public bool PhoneNumberConfirmed { get; set; }

		[Column("password_hash")]
		public string PasswordHash { get; set; }

		[Column("security_stamp")]
		public string SecurityStamp { get; set; }

		[Column("two_factor_enabled"), NotNull]
		public bool TwoFactorEnabled { get; set; }

		[Column("lockout_enabled"), NotNull]
		public bool LockoutEnabled { get; set; }

		[Column("lockout_end_date_utc")]
		public DateTime? LockoutEndDateUtc { get; set; }

		[Column("access_failed_count"), NotNull]
		public long AccessFailedCount { get; set; }
	}
}