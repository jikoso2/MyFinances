using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinances.Helpers
{
	public static class Helper
	{
		public static string MoneyFormat(double value)
		{
			return value.ToString("0.00") + " zł";
		}
	}
}
