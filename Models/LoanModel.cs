using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MyFinances.Data;

namespace MyFinances.Models
{
	public class LoanModel
	{
		[Required]
		[Range(1, 100000000, ErrorMessage = "Wysokość kredytu nie może być mniejsza od zera lub większa od 10 milionów")]
		public long Amount { get; set; } = 400000;

		[Required]
		[Range(0.01, 30, ErrorMessage = "Wysokość oprocentowania nie może być bardzo mała, ujemna lub większa od 30")]
		public double Percentage { get; set; } = 7.5;

		[Required]
		[Range(12, 420, ErrorMessage = "Długość kredytu musi zawierać się w przedziale od 12 do 420 miesięcy")]
		[LoanModelValidation.Duration]
		public int Duration { get; set; } = 360;

		public List<ExcessPayment> ExcessPayments { get; set; } = new List<ExcessPayment>();

		public Dictionary<int, double> VariableInterest { get; set; } = new Dictionary<int, double>();

		public double PercentageNumber { get { return Percentage / 100; } }

		public int MinDuration { get; set; } = 0;
	}

	internal class LoanModelValidation
	{
		internal class Duration : ValidationAttribute
		{
			protected override ValidationResult IsValid(object value, ValidationContext validationContext)
			{
				var loanModel = (LoanModel)validationContext.ObjectInstance;
				if (Double.Parse(value.ToString()) >= loanModel.MinDuration)
				{
					return null;
				}

				return new ValidationResult("Zadeklarowane zostały zmiany oprocentowania lub nadpłaty dla miesięcy powyżej trwania kredytu", new[] { validationContext.MemberName });
			}
		}
	}
}
