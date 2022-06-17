using MyFinances.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFinances.Helpers;
using System.Collections;

namespace MyFinances.Data
{
	public class PPKService
	{
		PPKModel PPKModel;

		public Task<PPK> GetPPKCalculatedAsync(PPKModel ppkModel)
		{
			PPKModel = ppkModel;
			var ppkResult = CalculatePPKResult();
			return Task.FromResult(ppkResult);
		}

		private PPK CalculatePPKResult()
		{
			var ppkResult = new PPK();

			var employeePayment = Math.Round(PPKModel.Amount * PPKModel.EmployeePercentage / 100, 2);
			var employeePaymentWithTax = Math.Round(employeePayment * 1.13, 2);
			var employerPayment = Math.Round(PPKModel.Amount * PPKModel.EmployerPercentage / 100, 2);

			var interestSum = 0.0;
			var finalAmount = 0.0;
			var employerAmount = 0.0;

			for (int i = 1; i <= PPKModel.Duration; i++)
			{
				var odsetki = Math.Round(finalAmount / 12 * PPKModel.DepositPercentage / 100, 2);
				interestSum += odsetki;
				employerAmount += employeePayment;
				finalAmount += Math.Round(odsetki + employeePayment + employerPayment, 2);
			}


			ppkResult.PPKInfo.Add(Tuple.Create("Miesięczny koszt pracownika", Helper.MoneyFormat(employeePaymentWithTax)));
			ppkResult.PPKInfo.Add(Tuple.Create("Miesięczna wysokość wpłaty pracownika", Helper.MoneyFormat(employeePayment)));
			ppkResult.PPKInfo.Add(Tuple.Create("Miesięczna wysokość wpłaty pracodawcy", Helper.MoneyFormat(employerPayment)));

			ppkResult.PPKInfo.Add(Tuple.Create("Ilość okresów", PPKModel.Duration.ToString()));
			ppkResult.PPKInfo.Add(Tuple.Create("Zgromadzony kapitał", Helper.MoneyFormat(finalAmount)));
			ppkResult.PPKInfo.Add(Tuple.Create("Wielkość odsetek w kapitale", Helper.MoneyFormat(interestSum)));

			if (!PPKModel.EarlyPayment)
			{
				var taxFromOdsetki = 0.0;
				if (interestSum > 0)
					taxFromOdsetki = Math.Round(interestSum * 0.19, 2);

				var amountToZUS = Math.Round(employeePayment * 0.3 * PPKModel.Duration, 2);
				var amountEarlyPayment = Math.Round(finalAmount - amountToZUS - taxFromOdsetki, 2);
				var totalProfit = amountEarlyPayment - (PPKModel.Duration * employeePaymentWithTax);

				ppkResult.PPKInfo.Add(Tuple.Create("Podatek od zgromadzonych odsetek", Helper.MoneyFormat(taxFromOdsetki)));
				ppkResult.PPKInfo.Add(Tuple.Create("Część odprowadzona do ZUSu", Helper.MoneyFormat(amountToZUS)));
				ppkResult.PPKInfo.Add(Tuple.Create("Kwota Wypłaty", Helper.MoneyFormat(amountEarlyPayment)));
				ppkResult.PPKInfo.Add(Tuple.Create("Zysk netto przy wcześniejszej wypłacie", Helper.MoneyFormat(totalProfit)));
			}

			return ppkResult;
		}

		public string GetInformationAboutPPK()
		{
			return HelperInformations.GetPPKInformation();
		}
	}
	public class PPK
	{
		public PPK()
		{
			this.PPKData = new PPKData();
			this.PPKInfo = new List<Tuple<string, string>>();
		}

		public PPKData PPKData { get; set; }
		public List<Tuple<string, string>> PPKInfo { get; set; }
	}

	public class PPKData
	{
		public string[] Head { get; set; }
	}
}
