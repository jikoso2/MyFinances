using MyFinances.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFinances.Helpers;

namespace MyFinances.Data
{
	public class DepositService
	{
		DepositModel DepositModel;
		public Task<Deposit> GetDepositAsync(DepositModel depositModel)
		{
			DepositModel = depositModel;
			var depositResult = CalculateDeposit();

			return Task.FromResult(depositResult);
		}

		private Deposit CalculateDeposit()
		{
			var depositResult = new Deposit(DepositModel);
			int periods = (int)Math.Floor((double)DepositModel.Duration / DepositModel.Period);

			var periodRows = new string[periods];
			var interestRows = new string[periods];
			var profitRows = new string[periods];

			double capital = DepositModel.Amount;
			double interestSum = 0;
			double interest;
			double interestWithoutTax;

			for (int i = 0; i < periods; i++)
			{
				periodRows[i] = (i + 1).ToString();
				interestWithoutTax = Math.Round(capital * DepositModel.PercentageNumber * DepositModel.Period / 365,2);

				if (DepositModel.BelkaTax)
				{
					interest = Math.Floor(interestWithoutTax * 0.81 * 100) - 1;
					interest = Math.Round(interest<0?0:interest / 100, 2);
					interestRows[i] = Helper.MoneyFormat(interest);
					interestSum += interest;
					if (DepositModel.Capitalization)
						capital += interest;
				}
				else
				{
					interestRows[i] = Helper.MoneyFormat(interestWithoutTax);
					interestSum += interestWithoutTax;
					if (DepositModel.Capitalization)
						capital += interestWithoutTax;
				}

				profitRows[i] = Helper.MoneyFormat(interestSum);
				
				
			}

			depositResult.DepositData.DepositColumn = new DepositColumn[3]
			{
				new DepositColumn() { Rows = periodRows },
				new DepositColumn() { Rows = interestRows },
				new DepositColumn() { Rows = profitRows }
			};

			depositResult.DepositInfo.Add(Tuple.Create("Kwota na lokacie", Helper.MoneyFormat(DepositModel.Amount)));
			depositResult.DepositInfo.Add(Tuple.Create("Ilość Okresów rozliczeniowych", periods.ToString()));
			//depositResult.DepositInfo.Add(Tuple.Create("Całkowita wartość odsetek", ));


			return depositResult;
		}
	}

	public class Deposit
	{
		public Deposit(DepositModel depositModel)
		{
			this.DepositData = new DepositResult();
			this.DepositData.Head = new string[3] { "Okres", depositModel.Capitalization?"Kapitalizowane Odsetki":"Wypłata", "Zysk przy wypłacie" };
			this.DepositInfo = new List<Tuple<string, string>>();
		}

		public int Duration { get; set; }
		public double Amount { get; set; }
		public double PercentageNumber { get; set; }
		public DepositResult DepositData { get; set; }
		public List<Tuple<string, string>> DepositInfo { get; set; }
	}

	public class DepositResult
	{
		public string[] Head { get; set; }
		public DepositColumn[] DepositColumn { get; set; }
	}

	public class DepositColumn
	{
		public string[] Rows { get; set; }
	}
}
