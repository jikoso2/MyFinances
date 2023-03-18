using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MyFinances.Models
{
	public class DepositAccountModel
	{
		[Required]
		[Range(0.0, 1000000, ErrorMessage = "Zgromadzony kapitał musi być dodatni")]
		public double StartAmount { get; set; } = Helpers.DefaultValue.DepositAccount.StartAmount;

		[Required]
		[Range(0.0, 1000000, ErrorMessage = "Zgromadzony kapitał Państwa musi być dodatni")]
		public double MonthlyPayment { get; set; } = Helpers.DefaultValue.DepositAccount.MonthlyPayment;

		[Required]
		[Range(0, 64, ErrorMessage = "Długość oszczędzania musi być pomiędzy 1 a 64 miesiącami")]
		public int Lenght { get; set; } = Helpers.DefaultValue.DepositAccount.Length;

		[Required]
		[Range(0, 15, ErrorMessage = "Wartość oprocentowania musi być większa od 0 i mniejsza od 15")]
		public double Percentage { get; set; } = Helpers.DefaultValue.DepositAccount.Percentage;

		public bool BelkaTax { get; set; } = true;

		public DepositAccountType DepositAccountType { get; set; } = DepositAccountType.Prosty;
	}

	public enum DepositAccountType
	{
		Prosty,
		Rozbudowany
	}
}
