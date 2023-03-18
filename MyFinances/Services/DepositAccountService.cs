using MyFinances.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFinances.Helpers;
using System.Collections;

namespace MyFinances.Data
{
	public class DepositAccountService
	{
		DepositAccountModel DepositAccountModel;

		public Task<DepositAccountResult> GetDepositAccountCalculationAsync(DepositAccountModel depositAccountModel)
		{
			DepositAccountModel = depositAccountModel;
			var depositAccountCalculation = CalculateDepositAccountResult();
			return Task.FromResult(depositAccountCalculation);
		}

		private DepositAccountResult CalculateDepositAccountResult()
		{
			var depositAccountCalculationResult = new DepositAccountResult();

			switch (DepositAccountModel.DepositAccountType)
			{
				case DepositAccountType.Prosty:
					CalculateSimple(depositAccountCalculationResult);
					break;
				case DepositAccountType.Rozbudowany:
					CalculateExtensive(depositAccountCalculationResult);
					break;
				default:
					break;
			}

			return depositAccountCalculationResult;
		}

		private void CalculateExtensive(DepositAccountResult depositAccountCalculationResult)
		{
			
		}

		private void CalculateSimple(DepositAccountResult depositAccountCalculationResult)
		{
			int periods = DepositAccountModel.Lenght;

			var periodRows = new string[periods];
			var interestRows = new string[periods];
			var profitRows = new string[periods];

			double startCapital = DepositAccountModel.StartAmount;
			double totalPayment = startCapital;
			double interestSum = 0;
			double interest;
			double interestWithoutTax;
			double capital = startCapital;

			for (int i = 0; i < periods; i++)
			{
				periodRows[i] = (i + 1).ToString();

				if (i > 0)
				{
					capital += DepositAccountModel.MonthlyPayment;
					totalPayment += DepositAccountModel.MonthlyPayment;
				}

				interestWithoutTax = Math.Round(capital * DepositAccountModel.Percentage / 12.0 / 100, 2);

				if (DepositAccountModel.BelkaTax)
				{
					interest = Math.Round(Math.Floor(interestWithoutTax * 0.81 * 100) / 100, 2);
					interestRows[i] = Helper.MoneyFormat(interest);
					interestSum += interest;
					capital += interest;
				}
				else
				{
					interestRows[i] = Helper.MoneyFormat(interestWithoutTax);
					interestSum += interestWithoutTax;
					capital += interestWithoutTax;
				}

				profitRows[i] = Helper.MoneyFormat(interestSum);
			}

			//depositResult.DepositData.DepositColumn = new DepositColumn[3]
			//{
			//	new DepositColumn() { Rows = periodRows },
			//	new DepositColumn() { Rows = interestRows },
			//	new DepositColumn() { Rows = profitRows }
			//};

			//depositResult.DepositInfo.Add(Tuple.Create("Kwota na lokacie", Helper.MoneyFormat(DepositModel.Amount)));
			//depositResult.DepositInfo.Add(Tuple.Create("Ilość okresów rozliczeniowych", periods.ToString()));
			//depositResult.DepositInfo.Add(Tuple.Create("Całkowita wartość odsetek", ));


			depositAccountCalculationResult.DepositAccountInfo.Add(Tuple.Create("Wpłacona kwota", Helper.MoneyFormat(totalPayment)));
			depositAccountCalculationResult.DepositAccountInfo.Add(Tuple.Create("Końcowa kwota", Helper.MoneyFormat(capital)));
			//depositAccountCalculationResult.DepositAccountInfo.Add(Tuple.Create("Zysk netto", "test"));
		}
	}
	public class DepositAccountResult
	{
		public DepositAccountResult()
		{
			this.DepositAccountInfo = new List<Tuple<string, string>>();
		}

		public List<Tuple<string, string>> DepositAccountInfo { get; set; }
	}
}
