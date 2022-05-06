using MyFinances.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinances.Data
{
	public class CalculatedLoan
	{
		public double Instalment { get { return _instalment; } set { _instalment = value; } }
		private double _instalment;

		public double TotalAmount { get { return _totalAmount; } }
		private double _totalAmount;

		public List<LoanRow> RepaymentSchedule { get { return _repaymentSchedule; } }
		private List<LoanRow> _repaymentSchedule = new List<LoanRow>();

		public void Calculate(LoanModel loanModel)
		{
			_instalment = Math.Round((loanModel.Amount * loanModel.PercentageNumber) / (12 * (1 - Math.Pow((12 / (12 + loanModel.PercentageNumber)), loanModel.Duration))),2);
			_totalAmount = _instalment * loanModel.Duration;
			_repaymentSchedule = CalculateRepaymentSchedule(loanModel);
		}

		private List<LoanRow> CalculateRepaymentSchedule(LoanModel loanModel)
		{
			var result = new List<LoanRow>();

			double capital = loanModel.Amount;
			double interest = Math.Round(capital * loanModel.PercentageNumber / 12,2);

			for (int i = 0; i < loanModel.Duration + 1; i++)
			{
				if (Instalment > capital + interest)
					Instalment = capital + interest;
				var row = new LoanRow() { Month = i + 1, Loan = Instalment, Interest = interest, Capital = capital };
				result.Add(row);
				capital = Math.Abs(Math.Round(capital - Instalment + interest,2));
				interest = Math.Abs(Math.Round(capital * loanModel.PercentageNumber / 12,2));
			}
			return result;
		}
	}

	public class LoanRow
	{
		public int Month { get; set; }
		public double Loan { get; set; }
		public double Interest { get; set; }
		public double Capital { get; set; }
	}
}
