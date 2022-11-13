﻿using MyFinances.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFinances.Helpers;
using System.Collections;

namespace MyFinances.Data
{
	public class PPKPayoutService
	{
		PPKPayoutModel PPKPayoutModel;

		public Task<PPKPayout> GetPPKPayoutCalculatedAsync(PPKPayoutModel ppkPayoutModel)
		{
			PPKPayoutModel = ppkPayoutModel;
			var ppkResult = CalculatePPKPayoutResult();
			return Task.FromResult(ppkResult);
		}

		private PPKPayout CalculatePPKPayoutResult()
		{
			var ppkResult = new PPKPayout();
			double amount = 0.0;

			switch (PPKPayoutModel.PayoutType)
			{
				case PayoutType.Całość:
					amount = PPKPayoutModel.Amount - ((1 + PPKPayoutModel.Percentage / 100) * PPKPayoutModel.CountryAmount);
					break;
				case PayoutType.Cześciami:
					amount = (PPKPayoutModel.EmployerAmount + PPKPayoutModel.EmployeeAmount) * (1 + PPKPayoutModel.Percentage / 100);
					break;
			}

			var EmployerAmount = amount / 7 * 3 * 0.7;
			var EmployeeAmount = amount / 7 * 4;

			var EmployerTax = 0.0;
			var EmployeeTax = 0.0;

			if (PPKPayoutModel.Percentage > 0)
			{
				EmployerTax = EmployerAmount / (1 + PPKPayoutModel.Percentage / 100) * (PPKPayoutModel.Percentage / 100) * 0.19;
				EmployeeTax = EmployeeAmount / (1 + PPKPayoutModel.Percentage / 100) * (PPKPayoutModel.Percentage / 100) * 0.19;
			}

			var totalPayout = EmployerAmount + EmployeeAmount - EmployerTax - EmployeeTax;

			ppkResult.PPKPayoutInfo.Add(Tuple.Create("Wypłata części pracownika", Helper.MoneyFormat(EmployeeAmount - EmployeeTax)));
			ppkResult.PPKPayoutInfo.Add(Tuple.Create("Wypłata części pracodawcy", Helper.MoneyFormat(EmployerAmount - EmployerTax)));
			ppkResult.PPKPayoutInfo.Add(Tuple.Create("Wypłata", Helper.MoneyFormat(totalPayout)));

			switch (PPKPayoutModel.PayoutType)
			{
				case PayoutType.Całość:
					break;
				case PayoutType.Cześciami:
					ppkResult.PPKPayoutInfo.Add(Tuple.Create("Koszt netto pracownika", Helper.MoneyFormat(PPKPayoutModel.EmployerAmount * 0.17 + PPKPayoutModel.EmployeeAmount)));
					ppkResult.PPKPayoutInfo.Add(Tuple.Create("Zysk netto pracownika", Helper.MoneyFormat(totalPayout - (PPKPayoutModel.EmployerAmount * 0.17 + PPKPayoutModel.EmployeeAmount))));
					break;
			}

			return ppkResult;
		}

		public string GetInformationAboutPPK()
		{
			return HelperInformations.GetPPKInformation();
		}
	}
	public class PPKPayout
	{
		public PPKPayout()
		{
			this.PPKData = new PPKPayoutData();
			this.PPKPayoutInfo = new List<Tuple<string, string>>();
		}

		public PPKPayoutData PPKData { get; set; }
		public List<Tuple<string, string>> PPKPayoutInfo { get; set; }
	}

	public class PPKPayoutData
	{
		public string[] Head { get; set; }
	}
}
