using MyFinances.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinances.Data
{
	public class CalculatedDebenture
	{
		public DebentureType Type { get { return _type; } }
		private DebentureType _type;

		public double TotalAmount { get { return _totalAmount; } }
		private double _totalAmount;

		public void Calculate(DebenturesModel debenturesModel)
		{
			_type = debenturesModel.Type;
			_totalAmount = debenturesModel.Amount * 100;
		}
	}
}
