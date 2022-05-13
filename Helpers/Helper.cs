using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinances.Helpers
{
	public static class Helper
	{
		public static string MoneyFormat(double value)
		{
			var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
			nfi.NumberGroupSeparator = " ";
			return value.ToString("#,0.00",nfi) + " zł";
		}

	}
	public static class DefaultValue
	{
		public static double OTSPercentage = 1.5;
		public static double DOSPercentage = 3;
	}
}
