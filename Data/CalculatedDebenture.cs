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

			switch (debenturesModel.Type)
			{
				case DebentureType.OTS:
					CalculateOTS();
					break;
				case DebentureType.DOS:
					CalculateDOS();
					break;
				case DebentureType.TOZ:
					CalculateTOZ();
					break;
				case DebentureType.COI:
					CalculateCOI();
					break;
				case DebentureType.EDO:
					CalculateEDO();
					break;
				default:
					break;
			}
			
		}

		private void CalculateEDO()
		{
			//throw new NotImplementedException();
		}

		private void CalculateCOI()
		{
			//throw new NotImplementedException();
		}

		private void CalculateTOZ()
		{
			//throw new NotImplementedException();
		}

		private void CalculateDOS()
		{
			//throw new NotImplementedException();
		}

		private void CalculateOTS()
		{
			//throw new NotImplementedException();
		}
	}
}
