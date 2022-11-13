using System;
using System.ComponentModel.DataAnnotations;

namespace MyFinances.Data.DataBase
{
	public class LoanCalculation
	{
		[Key]

		public int iid { get; set; }

		public long amount { get; set; }

		public int duration { get; set; }

		public double percentage { get; set; }

		public Guid fk_user_id { get; set; }
	}
}
