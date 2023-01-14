using MyFinances.Helpers;
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
		[Range(0.01, 100000000, ErrorMessage = "Kwota deponowana na lokacie musi być dodatnia i mniejsza od 100 milionów")]
		public double Amount { get; set; } = DefaultValue.Deposit.Amount;

		[Required]
		[Range(0.001, 30, ErrorMessage = "Wysokość oprocentowania nie może być mniejsza od zera lub większa od 30")]
		public double Percentage { get; set; } = DefaultValue.Deposit.Percentage;

		[Required]
		[Range(1, 365, ErrorMessage = "Długość deponowania gotówki musi zawierać się w przedziale od 1 do 365")]
		public int Duration { get; set; } = DefaultValue.Deposit.Duration;

		[Required]
		public TimeType DurationType { get; set; } = TimeType.Miesiąc;

		[Required]
		[Range(1, 365, ErrorMessage = "Okres kapitalizacji musi zawierać się w przedziale od 1 do 365")]
		public int Period { get; set; } = DefaultValue.Deposit.Period;

		public bool BelkaTax { get; set; } = true;

		public bool Capitalization { get; set; }

		public double PercentageNumber { get { return Percentage / 100; } }
	}

	public enum TimeType
	{
		Dzień,
		Miesiąc,
		Rok
	}
}
