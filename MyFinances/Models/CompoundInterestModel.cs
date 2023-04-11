using MyFinances.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static MyFinances.Components.Modals.CompoundInterest;

namespace MyFinances.Models
{
	public class CompoundInterestModel
	{
		[Required]
		[Range(0.001, 100, ErrorMessage = "Wysokość oprocentowania nie może być mniejsza od zera lub większa od 100")]
		public double Percentage { get; set; } = DefaultValue.Deposit.Percentage;

		[Required]
		[Range(1, 365, ErrorMessage = "Okres trwania nie powinien być większy od 365")]
		public int Duration { get; set; } = DefaultValue.Deposit.Duration;

		[Required]
		public TimeType TotalDurationType { get; set; } = TimeType.Miesiąc;

		[Required]
		public CapitalizationType CapitalizationDurationType { get; set; } = CapitalizationType.Miesięczna;

		[Required]
		public bool BelkaTax { get; set; } = false;

	}
}
