using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinances.Models
{
	class ExcessPaymentModel
	{
		[Required]
		[Range(1, 420, ErrorMessage = "Miesiąc musi być liczbą dodatnią")]
		[ExcessPaymentModelValidation.Month]
		public int Month { get; set; } = 12;

		[Required]
		[Range(1, 1000000000, ErrorMessage = "Nadpłacić można minimum jedną złotówke")]
		public double Amount { get; set; } = 1000;

		public int LoanDuration { get; set; } = 0;

	}

	internal class ExcessPaymentModelValidation
	{
		internal class Month : ValidationAttribute
		{
			protected override ValidationResult IsValid(object value, ValidationContext validationContext)
			{
				var excessPaymentModel = (ExcessPaymentModel)validationContext.ObjectInstance;
				if (Double.Parse(value.ToString()) < excessPaymentModel.LoanDuration)
				{
					return null;
				}
				if (Double.Parse(value.ToString()) == excessPaymentModel.LoanDuration)
				{
					return new ValidationResult("Nie można nadpłacić w ostatnim miesiącu", new[] { validationContext.MemberName });
				}

				return new ValidationResult("Nadpłacać można jedynie w miesiącach trwania kredytu", new[] { validationContext.MemberName });
			}
		}
	}
}
