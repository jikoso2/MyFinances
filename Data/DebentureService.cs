using MyFinances.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFinances.Helpers;

namespace MyFinances.Data
{
	public class DebentureService
	{
		DebentureModel DebentureModel;
		public Task<Debenture> GetDebentureAsync(DebentureModel debentureModel)
		{
			DebentureModel = debentureModel;
			var debentureResult = new Debenture(DebentureModel);

			switch (debentureModel.Type)
			{
				case DebentureType.OTS:
					CalculateOTS(debentureResult);
					break;
				case DebentureType.DOS:
					CalculateDOS(debentureResult);
					break;
				case DebentureType.TOZ:
					CalculateTOZ(debentureResult);
					break;
				case DebentureType.COI:
					CalculateCOI(debentureResult);
					break;
				case DebentureType.EDO:
					CalculateEDO(debentureResult);
					break;
				case DebentureType.ROR:
					CalculateRateIndexed(debentureResult);
					break;
				case DebentureType.DOR:
					CalculateRateIndexed(debentureResult);
					break;
				default:
					break;
			}

			return Task.FromResult(debentureResult);
		}

		private void CalculateDOS(Debenture debentureResult)
		{
			var totalValue = new double[3];
			Array.Fill(totalValue, DebentureModel.Amount * 100);
			var interestRate = new double[3];
			Array.Fill(interestRate, DebentureModel.DOSPercentage);
			var interestProfit = new double[3];
			var tax = new double[3];
			var totalProfit = new double[3];

			interestProfit[0] = 0;
			interestProfit[1] = totalValue[1] * interestRate[1] / 100;
			totalValue[2] += interestProfit[1];
			interestProfit[2] = totalValue[2] * interestRate[1] / 100;

			tax[0] = 0;
			tax[1] = DebentureModel.BelkaTax ? (Math.Floor((interestProfit[1] - 0.7) * 19) + 1) / 100 < 0.01 ? 0 : (Math.Floor((interestProfit[1] - 0.7) * 19) + 1) / 100 : 0;
			tax[2] = DebentureModel.BelkaTax ? (Math.Floor((interestProfit[1] + interestProfit[2]) * 19) + 1) / 100 < 0.01 ? 0 : (Math.Floor((interestProfit[1] + interestProfit[2]) * 19) + 1) / 100 : 0;

			totalProfit[0] = 0;
			totalProfit[1] = DebentureModel.BelkaTax ? interestProfit[1] - 0.7 - tax[1] : interestProfit[1] - 0.7;
			totalProfit[2] = DebentureModel.BelkaTax ? interestProfit[2] - tax[2] + interestProfit[1] : interestProfit[2] + interestProfit[1];


			var interestRateRows = interestRate.Select(a => Helper.PercentFormat(a)).ToArray();
			var yearRows = Enumerable.Range(0, 3).Select(a => a.ToString()).ToArray();
			var totalValueRows = totalValue.Select(a => Helper.MoneyFormat(a)).ToArray();
			var interestProfitRows = interestProfit.Select(a => Helper.MoneyFormat(a)).ToArray();
			var taxRows = tax.Select(a => Helper.MoneyFormat(a)).ToArray();
			var totalProfitRows = totalProfit.Select(a => Helper.MoneyFormat(a)).ToArray();

			debentureResult.DebentureData.DebentureColumns = new DebentureColumn[6]
			{
				new DebentureColumn() { Rows = yearRows },
				new DebentureColumn() { Rows = totalValueRows },
				new DebentureColumn() { Rows = interestRateRows },
				new DebentureColumn() { Rows = interestProfitRows },
				new DebentureColumn() { Rows = taxRows },
				new DebentureColumn() { Rows = totalProfitRows }
			};

			debentureResult.DebentureInfo.Add(Tuple.Create("Całkowity Zysk", totalProfitRows[2]));
		}

		private void CalculateTOZ(Debenture debentureResult)
		{
			var totalValue = new double[7];
			Array.Fill(totalValue, DebentureModel.Amount * 100);
			var interestRate = DebentureModel.TOZPercentage.Select(a => Math.Round(a / 100, 5)).ToList();
			interestRate.Add(0);
			var interestProfit = new double[7];
			var totalProfit = new double[7];
			var totalProfitRes = new double[7];
			var interestSum = new double[7];

			double calculatedTax = 0;


			for (int i = 0; i < 7; i++)
			{
				double profit = i <= 6 ? Math.Round(totalValue[i] * interestRate[i] / 2, 2) : 0;

				interestProfit[i] = profit;

				//interestSum[i + 1] = i > 0 ? interestSum[i - 1] + profit : profit;

				if (i < 6)
					totalProfit[i + 1] = i == 0 ? profit : totalProfit[i] + profit;

				if (DebentureModel.BelkaTax)
				{
					calculatedTax = (Math.Ceiling(Math.Max(totalProfit[i] - 0.70, 0) * 19)) / 100;
					if (i == 6)
						calculatedTax = Math.Ceiling(totalProfit[i] * 19) / 100;

				}
				totalProfitRes[i] = totalProfit[i] - calculatedTax;
				if (i < 6)
					totalProfitRes[i] = totalProfitRes[i] - 0.7 > 0 ? totalProfitRes[i] - 0.7 : 0;
			}

			var interestRateRows = interestRate.Select(a => Helper.PercentFormat(a * 100)).ToArray();
			var yearRows = Enumerable.Range(0, 7).Select(a => (Math.Round((double)a / 2, 1)).ToString()).ToArray();
			var totalValueRows = totalValue.Select(a => Helper.MoneyFormat(a)).ToArray();
			var interestProfitRows = interestProfit.Select(a => Helper.MoneyFormat(a)).ToArray();
			var totalProfitRows = totalProfitRes.Select(a => Helper.MoneyFormat(a)).ToArray();
			var interestSumRows = totalProfit.Select(a => Helper.MoneyFormat(a)).ToArray();

			debentureResult.DebentureData.DebentureColumns = new DebentureColumn[6]
			{
				new DebentureColumn() { Rows = yearRows },
				new DebentureColumn() { Rows = totalValueRows },
				new DebentureColumn() { Rows = interestRateRows },
				new DebentureColumn() { Rows = interestProfitRows },
				new DebentureColumn() { Rows = interestSumRows},
				new DebentureColumn() { Rows = totalProfitRows }
			};

			debentureResult.DebentureInfo.Add(Tuple.Create("Całkowity zysk", totalProfitRows[6]));
		}

		private void CalculateCOI(Debenture debentureResult)
		{
		}

		private void CalculateOTS(Debenture debentureResult)
		{
			var totalValue = new int[4];
			Array.Fill(totalValue, DebentureModel.Amount * 100);
			var interestRate = new double[4];
			Array.Fill(interestRate, DebentureModel.OTSPercentage);
			var interestProfit = new double[4];
			var tax = new double[4];
			var totalProfit = new double[4];


			for (int i = 0; i <= 3; i++)
			{
				var profit = Math.Round(DebentureModel.Amount * interestRate[i] * ((double)i / 12), 2);
				var calculatedTax = (Math.Floor(profit * 19) + 1) / 100 < 0.01 ? 0 : (Math.Floor(profit * 19) + 1) / 100;

				tax[i] = i == 3 && DebentureModel.BelkaTax ? calculatedTax : 0;
				interestProfit[i] = i == 3 ? profit : 0;
				totalProfit[i] = i == 3 ? profit - tax[i] : 0;
			}

			var interestRateRows = interestRate.Select(a => Helper.PercentFormat(a)).ToArray();
			var monthRows = Enumerable.Range(0, 4).Select(a => a.ToString()).ToArray();
			var totalValueRows = totalValue.Select(a => Helper.MoneyFormat(a)).ToArray();
			var interestProfitRows = interestProfit.Select(a => Helper.MoneyFormat(a)).ToArray();
			var taxRows = tax.Select(a => Helper.MoneyFormat(a)).ToArray();
			var totalProfitRows = totalProfit.Select(a => Helper.MoneyFormat(a)).ToArray();

			debentureResult.DebentureData.DebentureColumns = new DebentureColumn[6]
			{
				new DebentureColumn() { Rows = monthRows },
				new DebentureColumn() { Rows = totalValueRows },
				new DebentureColumn() { Rows = interestRateRows },
				new DebentureColumn() { Rows = interestProfitRows },
				new DebentureColumn() { Rows = taxRows },
				new DebentureColumn() { Rows = totalProfitRows }
			};

			debentureResult.DebentureInfo.Add(Tuple.Create("Całkowity zysk", totalProfitRows[3]));
		}

		private void CalculateEDO(Debenture debentureResult)
		{
			var totalValue = new double[11];
			Array.Fill(totalValue, DebentureModel.Amount * 100);
			var interestRate = DebentureModel.EDOPercentage.Select(a => Math.Round(a / 100, 5)).ToList();
			interestRate.Add(0);
			var interestProfit = new double[11];
			var tax = new double[11];
			var totalProfit = new double[11];
			var totalProfitRes = new double[11];
			double calculatedTax = 0.0;
			double earlyRedemptionFee = 2 * DebentureModel.Amount;

			for (int i = 0; i < 11; i++)
			{
				double profit = i <= 10 ? Math.Round(totalValue[i] * interestRate[i], 2) : 0;

				if (i < 10)
					totalValue[i + 1] = totalValue[i] + profit;

				interestProfit[i] = profit;

				if (i < 10)
					totalProfit[i + 1] = i == 0 ? profit : totalProfit[i] + profit;

				if (DebentureModel.BelkaTax)
				{
					calculatedTax = (Math.Ceiling(Math.Max(totalProfit[i] - earlyRedemptionFee, 0) * 19)) / 100;
					if (i == 10)
						calculatedTax = Math.Ceiling(totalProfit[i] * 19) / 100;
				}

				totalProfitRes[i] = totalProfit[i] - calculatedTax;
				if (i < 10)
					totalProfitRes[i] = totalProfitRes[i] - earlyRedemptionFee > 0 ? totalProfitRes[i] - earlyRedemptionFee : 0;

			}

			var yearRows = Enumerable.Range(0, 11).Select(a => a.ToString()).ToArray();
			var interestRateRows = interestRate.Select(a => Helper.PercentFormat(a * 100)).ToArray();
			var totalValueRows = totalValue.Select(a => Helper.MoneyFormat(a)).ToArray();
			var interestProfitRows = interestProfit.Select(a => Helper.MoneyFormat(a)).ToArray();
			var totalProfitRows = totalProfitRes.Select(a => Helper.MoneyFormat(a)).ToArray();

			debentureResult.DebentureData.DebentureColumns = new DebentureColumn[5]
			{
				new DebentureColumn() { Rows = yearRows },
				new DebentureColumn() { Rows = totalValueRows },
				new DebentureColumn() { Rows = interestRateRows },
				new DebentureColumn() { Rows = interestProfitRows },
				new DebentureColumn() { Rows = totalProfitRows }
			};

			debentureResult.DebentureInfo.Add(Tuple.Create("Całkowity zysk", totalProfitRows[totalProfitRows.Length - 1]));
		}

		private void CalculateRateIndexed(Debenture debentureResult)
		{
			var totalValue = DebentureModel.Amount * 100;
			var percentage = DebentureModel.Type == DebentureType.DOR ? DebentureModel.DORPercentage + 0.25 : DebentureModel.RORPercentage;
			var monthlyProfitPerDebenture = Math.Round(percentage / 12, 2);
			var monthlyTaxPerDebenture = (Math.Ceiling(monthlyProfitPerDebenture * 19)) / 100 < 0.01 ? 0 : (Math.Ceiling(monthlyProfitPerDebenture * 19)) / 100;
			var monthlyProfit = DebentureModel.BelkaTax ? monthlyProfitPerDebenture - monthlyTaxPerDebenture : monthlyProfitPerDebenture;
			var realPercentage = monthlyProfit >= 0.01 ? Math.Round(monthlyProfit * 12, 3) : 0;
			int totalLength = DebentureModel.Type == DebentureType.DOR ? 24 : 12;

			debentureResult.DebentureInfo.Add(Tuple.Create("Koszt obligacji", Helper.MoneyFormat(totalValue)));
			debentureResult.DebentureInfo.Add(Tuple.Create("Miesięczny zysk", Helper.MoneyFormat(monthlyProfit * DebentureModel.Amount)));
			debentureResult.DebentureInfo.Add(Tuple.Create($"Całkowity zysk ({totalLength} msc)", Helper.MoneyFormat(monthlyProfit * totalLength * DebentureModel.Amount)));

			if (DebentureModel.BelkaTax)
			{
				debentureResult.DebentureInfo.Add(Tuple.Create("Miesięczny podatek", Helper.MoneyFormat(monthlyTaxPerDebenture * DebentureModel.Amount)));
				debentureResult.DebentureInfo.Add(Tuple.Create("Całkowity podatek", Helper.MoneyFormat(monthlyTaxPerDebenture * 12 * DebentureModel.Amount)));
				debentureResult.DebentureInfo.Add(Tuple.Create("Rzeczywiste oprocentowanie bez podatku", Helper.PercentFormat(realPercentage)));
			}
			if (DebentureModel.Type == DebentureType.DOR)
			{
				debentureResult.DebentureInfo.Add(Tuple.Create("Obliczone oprocentowanie (DOR)", Helper.PercentFormat(percentage)));
			}
		}

		public string GetInformationAboutDebenture(DebentureType type)
		{
			return HelperInformations.GetInformation(type);
		}
	}
	public class Debenture
	{
		public Debenture(DebentureModel debentureModel)
		{
			this.DebentureData = new DebentureResult(debentureModel.Type);
			this.DebentureInfo = new();
		}
		public DebentureResult DebentureData { get; set; }
		public List<Tuple<string, string>> DebentureInfo { get; set; }
	}

	public class DebentureResult
	{
		public DebentureResult(DebentureType type)
		{
			switch (type)
			{
				case DebentureType.OTS:
					this.Head = new string[6] { "Miesiąc", "Całkowita wartość", "Oprocentowanie", "Odsetki", "Podatek", "Zysk netto" };
					break;
				case DebentureType.DOS:
					this.Head = new string[6] { "Rok", "Całkowita wartość", "Oprocentowanie", "Odsetki", "Podatek", "Zysk netto" };
					break;
				case DebentureType.TOZ:
					this.Head = new string[6] { "Rok", "Całkowita wartość", "Oprocentowanie", "Odsetki", "Suma odsetek", "Zysk netto przy wypłacie" };
					break;
				case DebentureType.COI:
					this.Head = new string[6] { "Rok", "Całkowita wartość", "Oprocentowanie", "Odsetki", "Podatek", "Zysk netto" };
					break;
				case DebentureType.EDO:
					this.Head = new string[5] { "Rok", "Całkowita wartość", "Oprocentowanie", "Odsetki", "Zysk przy wypłacie" };
					break;
				default:
					break;
			}
		}
		public string[] Head { get; set; }
		public DebentureColumn[] DebentureColumns { get; set; }
	}

	public class DebentureColumn
	{
		public string[] Rows { get; set; }
	}

}
