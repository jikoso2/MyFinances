using MyFinances.Models;
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
		public Task<Loan> GetLoanAsync(LoanModel loanModel)
		{
			LoanModel = loanModel;
			var loanResult = CalculateLoanResult();

			return Task.FromResult(loanResult);
		}
		private Loan CalculateLoanResult()
		{
			var loanResult = new Loan(LoanModel);

			var capital = new double[loanResult.Duration];
			var loan = new double[loanResult.Duration];
			var interest = new double[loanResult.Duration];
			var paymentSum = new double[loanResult.Duration];

			capital[0] = loanResult.Amount;

			for (int i = 0; i < loanResult.Duration; i++)
			{
				interest[i] = capital[i] * loanResult.PercentageNumber / 12;
				double calculatedLoan = CalculatedLoan(capital[i], loanResult.PercentageNumber, loanResult.Duration - i);

				if (LoanModel.ExcessPayments.Exists(x => x.Month == i + 1))
					calculatedLoan += LoanModel.ExcessPayments.Find(x => x.Month == i + 1).Amount;

				loan[i] = calculatedLoan;

				if (i < loanResult.Duration - 1)
				{
					capital[i + 1] = capital[i] - calculatedLoan + interest[i];

					if (capital[i + 1] <= 0)
						capital[i + 1] = 0;
				}

				paymentSum[i] = i != 0 ? paymentSum[i - 1] + loan[i] : 0;
			}

			var paymentsSumRows = paymentSum.Select(a => Helper.MoneyFormat(a)).ToArray();
			var interestRows = interest.Select(a => Helper.MoneyFormat(a)).ToArray();
			var totalValueRows = capital.Select(a => Helper.MoneyFormat(a)).ToArray();
			var loanRows = loan.Select(a => Helper.MoneyFormat(a)).ToArray();
			var monthRows = Enumerable.Range(1, loanResult.Duration).Select(a => a.ToString()).ToArray();

			loanResult.LoanData.LoanColumns = new LoanColumn[5]
			{
				new LoanColumn() { Rows = monthRows },
				new LoanColumn() { Rows = totalValueRows },
				new LoanColumn() { Rows = loanRows },
				new LoanColumn() { Rows = interestRows },
				new LoanColumn() { Rows = paymentsSumRows }
			};

			loanResult.TotalAdditionalPayment = Helper.MoneyFormat(loan.Sum() - loanResult.Amount);
			loanResult.TotalPaymentAmount = Helper.MoneyFormat(loan.Sum());
			loanResult.TotalAdditionalPaymentDouble = (loan.Sum()) - loanResult.Amount;
			loanResult.TotalPaymentAmountWithoutExcessPayment = Helper.MoneyFormat(CalculatedLoan(loanResult.Amount, loanResult.PercentageNumber, loanResult.Duration) * loanResult.Duration);
			loanResult.DifferenceBetweenOverpayments = Helper.MoneyFormat(CalculatedLoan(loanResult.Amount, loanResult.PercentageNumber, loanResult.Duration) * loanResult.Duration - loan.Sum());

			return loanResult;
		}

		private double CalculatedLoan(double capital, double percentage, int duration)
		{
			return Math.Round((capital * percentage) / (12 * (1 - Math.Pow((12 / (12 + percentage)), duration))), 2);
		}
	}

	public class Loan
	{
		public Loan(LoanModel loanModel)
		{
			this.Duration = loanModel.Duration;
			this.PercentageNumber = loanModel.PercentageNumber;
			this.Amount = loanModel.Amount;
			this.TotalAmount = Helper.MoneyFormat(loanModel.Amount);
			this.LoanData = new LoanResult();
			this.LoanData.Head = new string[5] { "Miesiąc", "Kapitał do spłaty", "Wysokość raty", "Kwota odsetek", "Suma wpłat" };
		}

		public int Duration { get; set; }
		public double Amount { get; set; }
		public double PercentageNumber { get; set; }
		public string TotalAmount { get; set; }
		public string TotalPaymentAmount { get; set; }
		public double TotalAdditionalPaymentDouble { get; set; }
		public string TotalAdditionalPayment { get; set; }
		public string TotalPaymentAmountWithoutExcessPayment { get; set; }
		public string DifferenceBetweenOverpayments { get; set; }
		public LoanResult LoanData { get; set; }
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
