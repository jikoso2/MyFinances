using MyFinances.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            public static List<double> EDOPercentage = new List<double>() { 6.9, 6, 5, 4, 3, 2.5, 2.5, 2.5, 2.5, 2.5 };
            public static double EDOAdditionalPercentage = 1.5;
            public static List<double> COIPercentage = new List<double>() { 6.65, 6, 5, 4 };
            public static double COIAdditionalPercentage = 1.25;
            public static double RORPercentage = 5.75;
            public static double DORPercentage = 6.25;
            public static double TOSPercentage = 6.5;
            public static double EarlyRedemptionCOI = 0.7;
            public static double EarlyRedemptionEDO = 0.7;
            public static double EarlyRedemptionTOS = 0.7;


            public static double GetEarlyRedemptionFee(DebentureType type)
            {
                switch (type)
                {
                    case DebentureType.COI:
                        return EarlyRedemptionCOI;
                    case DebentureType.EDO:
                        return EarlyRedemptionEDO;
                    case DebentureType.TOS:
                        return EarlyRedemptionTOS;
                    default:
                        return 0;
                }
            }
        }

        public class Loan
        {
            public static long Amount = 400000;
            public static double Percentage = 7;
            public static int Duration = 360;
        }

        public class Deposit
        {
            public static long Amount = 30000;
            public static double Percentage = 6;
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
