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
			var magicRelation = Math.Round(PPKModel.EmployerPercentage / (PPKModel.EmployeePercentage + PPKModel.EmployerPercentage), 2);

			var employeePayment = Math.Round(PPKModel.Amount * PPKModel.EmployeePercentage / 100, 2);
			var employerPayment = Math.Round(PPKModel.Amount * PPKModel.EmployerPercentage / 100, 2);
			var employeePaymentWithTax = Math.Round(employerPayment * 0.12 + employeePayment, 2);

			var interestSum = 0.0;
			var finalAmount = 0.0;
			var employerAmount = 0.0;
			var odsetki = 0.0;
			var allPayments = 0.0;

			for (int i = 1; i <= PPKModel.Duration; i++)
			{
				odsetki = Math.Round(finalAmount / 12 * PPKModel.DepositPercentage / 100, 2);
				interestSum += odsetki;
				employerAmount += employeePayment;
				finalAmount += Math.Round(odsetki + employeePayment + employerPayment, 2);
				allPayments += employeePayment + employerPayment;
			}


			ppkResult.PPKInfo.Add(Tuple.Create("Miesięczny koszt pracownika (+12% podatek dochodowy)", Helper.MoneyFormat(employeePaymentWithTax)));
			ppkResult.PPKInfo.Add(Tuple.Create("Miesięczna wysokość wpłaty pracownika", Helper.MoneyFormat(employeePayment)));
			ppkResult.PPKInfo.Add(Tuple.Create("Miesięczna wysokość wpłaty pracodawcy", Helper.MoneyFormat(employerPayment)));

			ppkResult.PPKInfo.Add(Tuple.Create("Ilość okresów", PPKModel.Duration.ToString()));
			ppkResult.PPKInfo.Add(Tuple.Create("Zgromadzony kapitał", Helper.MoneyFormat(finalAmount)));
			ppkResult.PPKInfo.Add(Tuple.Create("Wielkość odsetek w kapitale", Helper.MoneyFormat(interestSum)));

			if (PPKModel.EarlyPayment)
			{
				var amountToZUS = 0.0;
				var amountEarlyPayment = 0.0;
				var totalProfit = 0.0;
				if (PPKModel.DepositPercentage > 0)
				{
					var interestFromEmployer = Math.Round(interestSum * magicRelation * 0.7, 2);
					var interestFromEmployee = Math.Round(interestSum * (1 - magicRelation), 2);
					var interestFromEmployerWithoutTax = Math.Floor(interestFromEmployer * 0.81 * 100) / 100;
					var interestFromEmployeeWithoutTax = Math.Floor(interestFromEmployee * 0.81 * 100) / 100;
					amountToZUS = Math.Round(employerPayment * 0.3 * PPKModel.Duration, 2);
					amountEarlyPayment = Math.Round(allPayments - amountToZUS + interestFromEmployerWithoutTax + interestFromEmployeeWithoutTax, 2);
					totalProfit = amountEarlyPayment - (PPKModel.Duration * employeePaymentWithTax);
					ppkResult.PPKInfo.Add(Tuple.Create("Zgromadzone odsetki z wpłat pracownika minus podatek", Helper.MoneyFormat(interestFromEmployerWithoutTax)));
					ppkResult.PPKInfo.Add(Tuple.Create("Zgromadzone odsetki z wpłat pracodawcy minus podatek", Helper.MoneyFormat(interestFromEmployeeWithoutTax)));
				}
				else
				{
					amountToZUS = Math.Round(employerPayment * 0.3 * PPKModel.Duration, 2);
					amountEarlyPayment = Math.Round(finalAmount - amountToZUS, 2);
					totalProfit = amountEarlyPayment - (PPKModel.Duration * employeePaymentWithTax);
				}

				ppkResult.PPKInfo.Add(Tuple.Create("Część odprowadzona do ZUSu", Helper.MoneyFormat(amountToZUS)));
				ppkResult.PPKInfo.Add(Tuple.Create("Kwota wypłaty", Helper.MoneyFormat(amountEarlyPayment)));
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
