using MyFinances.Models;
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
					amount = PPKPayoutModel.Amount / (1 + PPKPayoutModel.Percentage / 100) - PPKPayoutModel.CountryAmount / (1 + PPKPayoutModel.Percentage / 100);
					break;
				case PayoutType.Cześciami:
					amount = PPKPayoutModel.EmployerAmount + PPKPayoutModel.EmployeeAmount;
					amount += PPKPayoutModel.EarlyPayment ? PPKPayoutModel.CountryAmount : 0;
					return ppkResult;
			}

			var EmployerAmount = amount / 7 * 3;
			var EmployeeAmount = amount / 7 * 4;

			var EmployerInterest = EmployerAmount * PPKPayoutModel.Percentage / 100 * 0.81;
			var EmployeeInterest = EmployeeAmount * PPKPayoutModel.Percentage / 100 * 0.81;

			var totalPayout = EmployerInterest + EmployeeInterest + EmployerAmount * 0.7 + EmployeeAmount;

			ppkResult.PPKPayoutInfo.Add(Tuple.Create("Wypłata części pracownika", Helper.MoneyFormat(EmployerAmount)));
			ppkResult.PPKPayoutInfo.Add(Tuple.Create("Wypłata części pracodawcy", Helper.MoneyFormat(EmployeeAmount * 0.7)));
			ppkResult.PPKPayoutInfo.Add(Tuple.Create("Wypłata części odsetkowej", Helper.MoneyFormat(EmployeeInterest + EmployerInterest)));
			ppkResult.PPKPayoutInfo.Add(Tuple.Create("Wypłata", Helper.MoneyFormat(totalPayout)));

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
