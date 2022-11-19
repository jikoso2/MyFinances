using System;
using System.ComponentModel.DataAnnotations;

namespace MyFinances.Data.DataBase
{
	public class UserAccount
	{
		[Key]
		public Guid id { get; set; }

		public string username { get; set; }

		public string password { get; set; }

		public string fullname { get; set; }

		public string email { get; set; }

		public string role { get; set; }

		public DateTime? last_login { get; set; }

		public string modified_by { get; set; }

		public DateTime? modified { get; set; }

		public DateTime? created { get; set; }

		public UserAccount()
		{
			id = Guid.NewGuid();
		}
	}
}
