using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinances.Models
{
	public class PPKModel
	{
		[Required]
		[Range(0.5, 4, ErrorMessage = "Wysokość oprocentowania nie może być mniejsza od zera lub większa od 30%")]
		public double EmployeePercentage { get; set; } = Helpers.DefaultValue.PPKCalc.EmployeePercentage;

		[Required]
		[Range(1.5, 4, ErrorMessage = "Wysokość oprocentowania nie może być mniejsza od zera lub większa od 30%")]
		public double EmployerPercentage { get; set; } = Helpers.DefaultValue.PPKCalc.EmployerPercentage;

		[Required]
		[Range(-10, 10, ErrorMessage = "Szacunkowe oprocentowanie funduszu musi zawierać się w przedziale -20% do 20%")]
		public double DepositPercentage { get; set; } = Helpers.DefaultValue.PPKCalc.DepositPercentage;

		[Required]
		[Range(1000, 10000000, ErrorMessage = "Wysokość wynagrodzenia brutto musi być większa od 1000 zł")]
		public long Amount { get; set; } = Helpers.DefaultValue.PPKCalc.Amount;

		[Required]
		[Range(1, 840, ErrorMessage = "Wpłaty na PPK mogą odbywać się od 1 do 840 miesięcy")]
		public int Duration { get; set; } = Helpers.DefaultValue.PPKCalc.Duration;

		[Required]
		public bool EarlyPayment { get; set; } = true;
	}
}
