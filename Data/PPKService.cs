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
			var employerPayment = Math.Round(PPKModel.Amount * PPKModel.EmployerPercentage / 100, 2);
			var paycheckFromEmployee = employeePayment;
			var paycheckFromEmployer = Math.Round(employerPayment * 0.7, 2);
			var paycheckToZUS = employerPayment - paycheckFromEmployer;
			var paycheckSum = paycheckFromEmployer + paycheckFromEmployee;

			ppkResult.PPKInfo.Add(Tuple.Create("Wysokość wpłaty pracownika", Helper.MoneyFormat(employeePayment)));
			ppkResult.PPKInfo.Add(Tuple.Create("Wysokość wpłaty pracodawcy", Helper.MoneyFormat(employerPayment)));
			ppkResult.PPKInfo.Add(Tuple.Create("Wypłata części pracowniczej po zerwaniu", Helper.MoneyFormat(paycheckFromEmployee)));
			ppkResult.PPKInfo.Add(Tuple.Create("Wypłata części pracodawcy po zerwaniu", Helper.MoneyFormat(paycheckFromEmployer)));
			ppkResult.PPKInfo.Add(Tuple.Create("Wpłata części pracodawcy po zerwaniu do ZUS-u", Helper.MoneyFormat(paycheckToZUS)));
			ppkResult.PPKInfo.Add(Tuple.Create("Sumaryczny zysk", Helper.MoneyFormat(paycheckSum)));

			return ppkResult;
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
