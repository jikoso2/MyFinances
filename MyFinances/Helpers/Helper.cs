using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

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
			return Math.Round(valueNumber, 5).ToString() + " %";
		}

		public static string ComputeHash(string input)
		{
			var sha = SHA256.Create();
			var byteArray = sha.ComputeHash(Encoding.UTF8.GetBytes(input));

			StringBuilder builder = new StringBuilder();
			foreach (var item in byteArray)
			{
				builder.Append(item.ToString("x2"));
			}

			return builder.ToString();
		}
	}

	public static class DefaultValue
	{

		public class Debenture
		{
			public static int Amount = 1;
			public static double OTSPercentage = 3;
			public static double DOSPercentage = 3;
			public static List<double> TOZPercentage = new List<double>() { 3, 2, 4, 3, 2, 4 };
			public static List<double> EDOPercentage = new List<double>() { 6.75, 16.1, 13.65, 13.65, 13.65, 13.65, 13.65, 13.65, 13.65, 13.65 };
			public static double EDOAdditionalPercentage = 1.25;
			public static List<double> COIPercentage = new List<double>() { 6.5, 10, 7, 4 };
			public static double COIAdditionalPercentage = 1;
			public static double RORPercentage = 6.5;
			public static double DORPercentage = 6.75;
			public static double TOSPercentage = 6.5;
		}

		public class Loan
		{
			public static long Amount = 400000;
			public static double Percentage = 9;
			public static int Duration = 360;
		}

		public class Deposit
		{
			public static long Amount = 30000;
			public static double Percentage = 8;
			public static int Duration = 3;
			public static int Period = 3;
		}

		public class PPKCalc
		{
			public static double EmployeePercentage = 2;
			public static double EmployerPercentage = 1.5;
			public static double DepositPercentage = 2;
			public static long Amount = 5000;
			public static int Duration = 12;
		}

		public class PPKPayout
		{
			public static double Amount = 2000;
			public static double CountryAmount = 250;
			public static double EmployeeAmount = 375;
			public static double EmployerAmount = 500;
			public static double Percentage = 0;
		}

		public class DepositAccount
		{
			public static double StartAmount = 10000;
			public static double Percentage = 3;
			public static double MonthlyPayment = 500;
			public static int Length = 6;
		}
	}
}
