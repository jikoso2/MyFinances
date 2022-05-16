using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinances.Models
{
	public class PeriodicExcessPaymentModel
	{
		public PeriodicExcessPaymentModel() { }
		public PeriodicExcessPaymentModel(PeriodicExcessPaymentModel model)
		{
			this.Amount = model.Amount;
			this.EndMonth = model.EndMonth;
			this.LoanDuration = model.LoanDuration;
			this.StartMonth = model.StartMonth;
		}

		[Required]
		[Range(1, 420, ErrorMessage = "Miesiąc musi być liczbą dodatnią")]
		[PeriodicExcessPaymentModelValidation.StartMonth]
		public int StartMonth { get; set; } = 12;

		[Required]
		[Range(1, 420, ErrorMessage = "Miesiąc musi być liczbą dodatnią")]
		[PeriodicExcessPaymentModelValidation.EndMonth]
		public int EndMonth { get; set; } = 24;

		[Required]
		[Range(1, 1000000000, ErrorMessage = "Nadpłacać można minimum jedną złotówke")]
		public double Amount { get; set; } = 1000;

		public int LoanDuration { get; set; } = 0;
	}

	internal class PeriodicExcessPaymentModelValidation
	{
		internal class EndMonth : ValidationAttribute
		{
			protected override ValidationResult IsValid(object value, ValidationContext validationContext)
			{
				var periodicExcessPaymentModel = (PeriodicExcessPaymentModel)validationContext.ObjectInstance;
				if (Double.Parse(value.ToString()) <= periodicExcessPaymentModel.LoanDuration)
				{
					return null;
				}

				return new ValidationResult("Nadpłacać można jedynie w miesiącach trwania kredytu", new[] { validationContext.MemberName });
			}
		}

		internal class StartMonth : ValidationAttribute
		{
			protected override ValidationResult IsValid(object value, ValidationContext validationContext)
			{
				var periodicExcessPaymentModel = (PeriodicExcessPaymentModel)validationContext.ObjectInstance;
				if (Double.Parse(value.ToString()) < periodicExcessPaymentModel.EndMonth)
				{
					return null;
				}

				return new ValidationResult("Nadpłacać można jedynie w miesiącach trwania kredytu", new[] { validationContext.MemberName });
			}
		}
	}
}
