using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MyFinances.Data;
using MyFinances.Helpers;

namespace MyFinances.Models
{
	public class LoanModel
	{
		[Required]
		[Range(1, 10000000, ErrorMessage = "Wysokość kredytu nie może być mniejsza od złotówki lub większa od 10 milionów złotych")]
		public long Amount { get; set; } = DefaultValue.Loan.Amount;

		[Required]
		[Range(0.01, 30, ErrorMessage = "Wysokość oprocentowania nie może być ujemna, mniejsza od 0,01 % lub większa od 30 %")]
		public double Percentage { get; set; } = DefaultValue.Loan.Percentage;

		[Required]
		[Range(12, 420, ErrorMessage = "Długość kredytu musi zawierać się w przedziale od 12 do 420 miesięcy")]
		[LoanModelValidation.Duration]
		public int Duration { get; set; } = DefaultValue.Loan.Duration;

		public List<ExcessPayment> ExcessPayments { get; set; } = new List<ExcessPayment>();

		public Dictionary<int, double> VariableInterest { get; set; } = new Dictionary<int, double>();

		public List<PeriodicExcessPaymentModel> PeriodicExcessPayments { get; set; } = new List<PeriodicExcessPaymentModel>();

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
				if (Double.Parse(value.ToString()) > loanModel.MinDuration)
				{
					return null;
				}
				if (Double.Parse(value.ToString()) == loanModel.MinDuration && loanModel.ExcessPayments.Count == 0)
				{
					return null;
				}

				return new ValidationResult("Zadeklarowane zostały zmiany oprocentowania lub nadpłaty dla miesięcy powyżej trwania kredytu", new[] { validationContext.MemberName });
			}
		}
	}
}
