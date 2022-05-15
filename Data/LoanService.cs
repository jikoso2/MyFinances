﻿using MyFinances.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFinances.Helpers;
using System.Collections;

namespace MyFinances.Data
{
	public class LoanService
	{
		LoanModel LoanModel;

		public Task<Loan> GetCalculatedLoanAsync(LoanModel loanModel)
		{
			LoanModel = loanModel;
			var loanResult = CalculateLoanResult();
			return Task.FromResult(loanResult);
		}

		private Loan CalculateLoanResult()
		{
			var loanResult = new Loan(LoanModel);

			var capital = new double[LoanModel.Duration];
			var instalment = new double[LoanModel.Duration];
			var interest = new double[LoanModel.Duration];
			var interestPercentage = GetVariableInterestPercentage();
			var paymentSum = new double[LoanModel.Duration];

			capital[0] = LoanModel.Amount;

			for (int i = 0; i < LoanModel.Duration; i++)
			{
				interest[i] = capital[i] * interestPercentage[i] / 12;
				double calculatedLoan = CalculatedConstantLoan(capital[i], interestPercentage[i], LoanModel.Duration - i);

				if (LoanModel.ExcessPayments.Exists(x => x.Month == i + 1))
					calculatedLoan += LoanModel.ExcessPayments.Find(x => x.Month == i + 1).Amount;

				instalment[i] = calculatedLoan;

				if (i < LoanModel.Duration - 1)
				{
					capital[i + 1] = capital[i] - calculatedLoan + interest[i];

					if (capital[i + 1] <= 0)
						capital[i + 1] = 0;
				}

				paymentSum[i] = i != 0 ? paymentSum[i - 1] + instalment[i] : instalment[i];
			}

			var paymentsSumRows = paymentSum.Select(a => Helper.MoneyFormat(a)).ToArray();
			var interestRows = interest.Select(a => Helper.MoneyFormat(a)).ToArray();
			var totalValueRows = capital.Select(a => Helper.MoneyFormat(a)).ToArray();
			var instalmentRows = instalment.Select(a => Helper.MoneyFormat(a)).ToArray();
			var monthRows = Enumerable.Range(1, LoanModel.Duration).Select(a => a.ToString()).ToArray();
			var interestPercentageRows = interestPercentage.Select(a => (Helper.PercentFormat(Math.Round(a * 100, 2))).ToString()).ToArray();

			loanResult.LoanData.LoanColumns = new LoanColumn[6]
			{
				new LoanColumn() { Rows = monthRows },
				new LoanColumn() { Rows = interestPercentageRows },
				new LoanColumn() { Rows = totalValueRows },
				new LoanColumn() { Rows = instalmentRows },
				new LoanColumn() { Rows = interestRows },
				new LoanColumn() { Rows = paymentsSumRows }
			};

			loanResult.LoanInfo.Add(Tuple.Create("Kwota zaciągniętego kredytu", Helper.MoneyFormat(LoanModel.Amount)));

			var TotalAdditionalPayment = instalment.Sum() - LoanModel.Amount;
			loanResult.LoanInfo.Add(Tuple.Create("Całkowita wartość odsetek", Helper.MoneyFormat(TotalAdditionalPayment)));
			loanResult.TotalAdditionalPaymentToPieChart = TotalAdditionalPayment;

			if (LoanModel.ExcessPayments.Count > 0)
			{
				loanResult.LoanInfo.Add(Tuple.Create("Suma nadpłat", Helper.MoneyFormat(LoanModel.ExcessPayments.Sum(a => a.Amount))));
			}

			if (LoanModel.ExcessPayments.Count > 0 && LoanModel.VariableInterest.Count == 0)
			{
				var DifferenceBetweenOverpayments = CalculatedConstantLoan(LoanModel.Amount, LoanModel.PercentageNumber, LoanModel.Duration) * LoanModel.Duration - instalment.Sum();
				loanResult.LoanInfo.Add(Tuple.Create("Różnica wpłat dla nadpłacanego kredytu", Helper.MoneyFormat(DifferenceBetweenOverpayments)));

				var TotalPaymentAmountWithoutExcessPayment = CalculatedConstantLoan(LoanModel.Amount, LoanModel.PercentageNumber, LoanModel.Duration) * LoanModel.Duration;
				loanResult.LoanInfo.Add(Tuple.Create("Kwota kredytu bez nadpłacania", Helper.MoneyFormat(TotalPaymentAmountWithoutExcessPayment)));
			}

			var TotalPaymentAmount = instalment.Sum();
			loanResult.LoanInfo.Add(Tuple.Create("Całkowity koszt kredytu", Helper.MoneyFormat(TotalPaymentAmount)));

			if (LoanModel.ExcessPayments.Count == 0 && LoanModel.VariableInterest.Count > 0)
			{
				loanResult.LoanInfo.Add(Tuple.Create("Całkowita kwota kredytu bez zmiany oprocentowania", Helper.MoneyFormat(CalculatedConstantLoan(LoanModel.Amount, LoanModel.PercentageNumber, LoanModel.Duration) * LoanModel.Duration)));
			}



			return loanResult;
		}

		private double[] GetVariableInterestPercentage()
		{
			var result = new double[LoanModel.Duration];
			Array.Fill(result, Math.Round(LoanModel.PercentageNumber, 4));
			foreach (var item in LoanModel.VariableInterest)
			{
				Array.Fill(result, Math.Round(item.Value / 100, 4), item.Key - 1, LoanModel.Duration - item.Key + 1);
			}
			return result;
		}

		private double CalculatedConstantLoan(double capital, double percentage, int duration)
		{
			return Math.Round((capital * percentage) / (12 * (1 - Math.Pow((12 / (12 + percentage)), duration))), 2);
		}
	}

	public class Loan
	{
		public Loan(LoanModel loanModel)
		{
			this.LoanData = new LoanResult();
			this.LoanData.Head = new string[6] { "Miesiąc", "Oprocentowanie", "Kapitał do spłaty", "Wysokość raty", "Kwota odsetek", "Suma wpłat" };
			this.LoanInfo = new List<Tuple<string, string>>();
		}

		public double TotalAdditionalPaymentToPieChart { get; set; }
		public LoanResult LoanData { get; set; }
		public List<Tuple<string, string>> LoanInfo { get; set; }
	}

	public class LoanResult
	{
		public string[] Head { get; set; }
		public LoanColumn[] LoanColumns { get; set; }
	}

	public class LoanColumn
	{
		public string[] Rows { get; set; }
	}

	public class ExcessPayment
	{
		public int Month { get; set; }
		public double Amount { get; set; }
	}
}
