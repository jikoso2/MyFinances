using MyFinances.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinances.Data
{
	public class CalculatedLoan
	{
		public double Instalment{ get {return _instalment; } }
		private double _instalment;
		public double TotalAmount { get { return _totalAmount; } }
		private double _totalAmount;
		public void Calculate(LoanModel loanModel)
		{
			_instalment = (loanModel.Amount * loanModel.PercentageNumber)/(12* (1 - Math.Pow((12/(12+loanModel.PercentageNumber)),loanModel.Duration)));
			_totalAmount = _instalment*loanModel.Duration;
		}
	}
}
