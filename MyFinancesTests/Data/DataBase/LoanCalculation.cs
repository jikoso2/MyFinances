using System;
using System.ComponentModel.DataAnnotations;

namespace MyFinances.Data.DataBase
{
	public class LoanCalculation
	{
		[Key]

		public int iid { get; set; }

		[Required]
		[Range(1, 10000000, ErrorMessage = "Wysokość kredytu nie może być mniejsza od złotówki lub większa od 10 milionów złotych")]
		public long amount { get; set; }

		[Required]
		[Range(0.01, 30, ErrorMessage = "Wysokość oprocentowania nie może być ujemna, mniejsza od 0,01 % lub większa od 30 %")]
		public double percentage { get; set; }

		[Required]
		[Range(12, 420, ErrorMessage = "Długość kredytu musi zawierać się w przedziale od 12 do 420 miesięcy")]
		public int duration { get; set; }

		public Guid fk_user_id { get; set; }
	}
}
