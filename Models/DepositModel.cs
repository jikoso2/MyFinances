using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinances.Models
{
	public class DepositModel
	{
		[Required]
		[Range(0.01, 100000000, ErrorMessage = "Kwota deponowana na lokacie musi być dodatnia")]
		public long Amount { get; set; } = 10000;

		[Required]
		[Range(0.001, 30, ErrorMessage = "Wysokość oprocentowania nie może być mniejsza od zera lub większa od 30")]
		public double Percentage { get; set; } = 3.5;

		[Required]
		[Range(1, 365, ErrorMessage = "Długość deponowania gotówki musi zawierać się w przedziale od 1 do 365")]
		public int Duration { get; set; } = 30;

		[Required]
		public TimeType DurationType { get; set; }

		[Required]
		[Range(1, 365, ErrorMessage = "Okres kapitalizacji musi zawierać się w przedziale od 1 do 365")]
		public int Period { get; set; } = 30;

		[Required]
		public TimeType PeriodType { get; set; }

		public bool BelkaTax { get; set; }

		public bool Capitalization { get; set; }

		public double PercentageNumber { get { return Percentage / 100; } }
	}
	public enum TimeType
	{
		Day,
		Month,
		Year
	}
}
