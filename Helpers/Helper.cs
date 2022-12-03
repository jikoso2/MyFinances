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
			return value.ToString("#,0.00", nfi) + " zł";
		}

		public static string PercentFormat(double valueNumber)
		{
			return Math.Round(valueNumber,5).ToString() + " %";
		}

	}

	public static class DefaultValue
	{
		public static int Amount = 1;
		public static double OTSPercentage = 3;
		public static double DOSPercentage = 3;
		public static List<double> TOZPercentage = new List<double>() { 3, 2, 4, 3, 2, 4 };
		public static List<double> EDOPercentage = new List<double>() { 6.75, 16.1, 13.65, 13.65, 13.65, 13.65, 13.65, 13.65, 13.65, 13.65 };
		public static double EDOAdditionalPercentage = 1.25;
		public static List<double> COIPercentage = new List<double>() { 6.5, 10, 7, 4};
		public static double RORPercentage = 6.5;
		public static double DORPercentage = 6.75;
		public static double TOSPercentage = 6.5;
	}
}
