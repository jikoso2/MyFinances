using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MyFinances.Models
{
	public class PPKPayoutModel
	{
		[Required]
		[Range(0.0, 1000000, ErrorMessage = "Zgromadzony kapitał musi być dodatni")]
		public double Amount { get; set; } = Helpers.DefaultValue.PPKPayout.Amount;

		[Range(0.0, 1000000, ErrorMessage = "Zgromadzony kapitał Państwa musi być dodatni")]
		public double CountryAmount { get; set; } = Helpers.DefaultValue.PPKPayout.CountryAmount;

		[Range(0.0, 1000000, ErrorMessage = "Zgromadzony kapitał pracownika musi być dodatni")]
		public double EmployeeAmount { get; set; } = Helpers.DefaultValue.PPKPayout.EmployeeAmount;

		[Range(0.0, 10000000, ErrorMessage = "Zgromadzony kapitał pracodawcy musi być dodatni")]
		public double EmployerAmount { get; set; } = Helpers.DefaultValue.PPKPayout.EmployerAmount;

		[Required]
		[Range(-30, 30, ErrorMessage = "Procent od -30 do 30")]
		public double Percentage { get; set; } = Helpers.DefaultValue.PPKPayout.Percentage;

		[Required]
		public bool EarlyPayment { get; set; } = true;

		[Required]
		public PayoutType PayoutType { get; set; } = PayoutType.Całość;
	}

	public enum PayoutType
	{
		Całość,
		Cześciami
	}
}
