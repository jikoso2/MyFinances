using MyFinances.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFinances.Helpers;

namespace MyFinances.Data
{
	public class LoanService
	{
		LoanModel LoanModel;
		public Task<Loan> GetLoanAsync(LoanModel loanModel)
		{
			LoanModel = loanModel;
			var loanResult = CalculateLoan();

			return Task.FromResult(loanResult);
		}

		private Loan CalculateLoan()
		{
			var loanResult = new Loan(LoanModel);

			var monthRows = new string[loanResult.Duration + 1];
			var totalValueRows = new string[loanResult.Duration + 1];
			var loanRows = new string[loanResult.Duration + 1];
			var interestRows = new string[loanResult.Duration + 1];
			var paymentsSum = new string[loanResult.Duration + 1];

			double loan = Math.Round((loanResult.Amount * loanResult.PercentageNumber) / (12 * (1 - Math.Pow((12 / (12 + loanResult.PercentageNumber)), loanResult.Duration))), 2);
			double capital = loanResult.Amount;
			double paymentSum = 0;

			for (int i = 1; i <= loanResult.Duration; i++)
			{
				double odsetki = capital * loanResult.PercentageNumber / 12;

				monthRows[i] = i.ToString();
				loanRows[i] = i==loanResult.Duration? Helper.MoneyFormat(capital+odsetki) : Helper.MoneyFormat(loan);
				interestRows[i] = Helper.MoneyFormat(odsetki);
				totalValueRows[i] = Helper.MoneyFormat(capital);
				paymentSum += loan;
				paymentsSum[i] = Helper.MoneyFormat(paymentSum);
				capital -= (loan - odsetki);
			}

			loanResult.LoanData.LoanColumns = new LoanColumn[5]
			{
				new LoanColumn() { Rows = monthRows },
				new LoanColumn() { Rows = totalValueRows },
				new LoanColumn() { Rows = loanRows },
				new LoanColumn() { Rows = interestRows },
				new LoanColumn() { Rows = paymentsSum }
			};

			loanResult.TotalAdditionalPayment = Helper.MoneyFormat((loanResult.Duration * loan) - loanResult.Amount);
			loanResult.TotalPaymentAmount = Helper.MoneyFormat(loanResult.Duration * loan);

			return loanResult;
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
		public string TotalAdditionalPayment { get; set; }
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
}
