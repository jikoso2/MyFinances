using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinances.Models
{
	class VariableInterestModel
	{
		[Required]
		[Range(1, 420, ErrorMessage = "Miesiąc musi być liczbą dodatnią")]
		[VariableInterestModelValidation.Month]
		public int Month { get; set; } = 12;

		[Required]
		[Range(0.01, 30, ErrorMessage = "Wysokość oprocentowania nie może być bardzo mała, ujemna lub większa od 30")]
		public double Percentage { get; set; } = 9;

		public int LoanDuration { get; set; } = 0;

	}

	internal class VariableInterestModelValidation
	{
		internal class Month : ValidationAttribute
		{
			protected override ValidationResult IsValid(object value, ValidationContext validationContext)
			{
				var variableInterestModel = (VariableInterestModel)validationContext.ObjectInstance;
				if (Double.Parse(value.ToString()) <= variableInterestModel.LoanDuration)
				{
					return null;
				}

				return new ValidationResult("Zmiana oprocentowania może nastąpić jedynie w miesiącach trwania kredytu", new[] { validationContext.MemberName });
			}
		}
	}
}
