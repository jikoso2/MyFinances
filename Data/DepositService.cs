﻿using MyFinances.Models;
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

			depositResult.NumberOfPeriods = periods.ToString();
			return depositResult;
		}
	}

	public class Deposit
	{
		public Deposit(DepositModel depositModel)
		{
			this.Duration = depositModel.Duration;
			this.PercentageNumber = depositModel.PercentageNumber;
			this.Amount = depositModel.Amount;
			this.TotalAmount = Helper.MoneyFormat(depositModel.Amount);
			this.DepositData = new DepositResult();
			this.DepositData.Head = new string[3] { "Okres", depositModel.Capitalization?"Kapitalizowane Odsetki":"Wypłata", "Zysk przy wypłacie" };
		}

		public int Duration { get; set; }
		public double Amount { get; set; }
		public double PercentageNumber { get; set; }
		public string TotalAmount { get; set; }
		public string NumberOfPeriods { get; set; }
		public string TotalAdditionalPayment { get; set; }
		public DepositResult DepositData { get; set; }
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
