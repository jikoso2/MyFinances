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
				case DebentureType.TOS:
					CalculateTOS(debentureResult);
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
			var totalValue = new double[5];
			Array.Fill(totalValue, DebentureModel.Amount * 100);
			var interestRate = DebentureModel.COIPercentage.Select(a => Math.Round(a / 100, 5)).ToList();
			interestRate.Add(0);
			var interestProfit = new double[5];
			var totalProfit = new double[5];
			var totalProfitRes = new double[5];
			var interestSum = new double[5];

			double calculatedTax = 0;


			for (int i = 0; i < 5; i++)
			{
				double profit = i <= 4 ? Math.Round(totalValue[i] * interestRate[i], 2) : 0;

				interestProfit[i] = profit;

				if (i < 4)
					totalProfit[i + 1] = i == 0 ? profit : totalProfit[i] + profit;

				if (DebentureModel.BelkaTax)
				{
					calculatedTax = (Math.Ceiling(Math.Max(totalProfit[i] - 0.70, 0) * 19)) / 100;
					if (i == 4)
						calculatedTax = Math.Ceiling(totalProfit[i] * 19) / 100;
				}

				totalProfitRes[i] = totalProfit[i] - calculatedTax;
				if (i < 4)
					totalProfitRes[i] = totalProfitRes[i] - 0.7 > 0 ? totalProfitRes[i] - 0.7 : 0;
			}

			var interestRateRows = interestRate.Select(a => Helper.PercentFormat(a * 100)).ToArray();
			var yearRows = Enumerable.Range(0, 5).Select(a => a.ToString()).ToArray();
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

			debentureResult.DebentureInfo.Add(Tuple.Create("Całkowity zysk", totalProfitRows[4]));
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
				var profitPerDebenture = Math.Round(100 * interestRate[i] / 100 * ((double)i / 12), 2);
				var profit = profitPerDebenture * DebentureModel.Amount;
				var calculatedTaxPerDebenture = (Math.Floor(profitPerDebenture * 19) + 1) / 100 < 0.01 ? 0 : (Math.Floor(profitPerDebenture * 19) + 1) / 100;
				var calculatedTax = calculatedTaxPerDebenture * DebentureModel.Amount;

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
				if (i >= 1 && i < 10)
					interestRate[i] = interestRate[i] <= 0 ? DebentureModel.EDOAdditionalPercentage / 100 : DebentureModel.EDOAdditionalPercentage / 100 + interestRate[i];

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
			var percentage = DebentureModel.Type == DebentureType.DOR ? DebentureModel.DORPercentage : DebentureModel.RORPercentage;
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

		private void CalculateTOS(Debenture debentureResult)
		{
			var percentage = DebentureModel.TOSPercentage / 100;
			var percentageCompound = Math.Round(((Math.Pow((1 + percentage), 3) - 1)) / 3, 4);

			var totalValue = new double[4];
			Array.Fill(totalValue, DebentureModel.Amount * 100);
			var interestRate = DebentureModel.TOSPercentage / 100;
			var interestRates = new double[4];
			Array.Fill(interestRates, DebentureModel.TOSPercentage / 100);
			var interestProfit = new double[4];
			var totalProfit = new double[4];
			var totalProfitRes = new double[4];
			var interestSum = new double[4];

			double calculatedTax = 0;

			for (int i = 0; i < 4; i++)
			{
				double profit = i <= 3 ? Math.Round(totalValue[i] * interestRate, 2) : 0;

				if (i < 3)
					interestProfit[i] = profit;
				else
					interestProfit[i] = 0;

				if (i < 3)
					totalProfit[i + 1] = i == 0 ? profit : totalProfit[i] + profit;

				if (DebentureModel.BelkaTax)
				{
					calculatedTax = (Math.Ceiling(Math.Max(totalProfit[i] - 0.70, 0) * 19)) / 100;
					if (i == 3)
						calculatedTax = Math.Ceiling(totalProfit[i] * 19) / 100;

				}
				totalProfitRes[i] = totalProfit[i] - calculatedTax;
				if (i < 3)
					totalProfitRes[i] = totalProfitRes[i] - 0.7 > 0 ? totalProfitRes[i] - 0.7 : 0;

				if (i > 0)
					totalValue[i] = totalValue[i - 1] + profit;
			}

			var yearRows = Enumerable.Range(0, 4).Select(a => (Math.Round((double)a, 1)).ToString()).ToArray();
			var totalValueRows = totalValue.Select(a => Helper.MoneyFormat(a)).ToArray();
			var totalRateRows = interestRates.Select(a => Helper.PercentFormat(a)).ToArray();
			var interestProfitRows = interestProfit.Select(a => Helper.MoneyFormat(a)).ToArray();
			var totalProfitRows = totalProfitRes.Select(a => Helper.MoneyFormat(a)).ToArray();
			var interestSumRows = totalProfit.Select(a => Helper.MoneyFormat(a)).ToArray();

			debentureResult.DebentureData.DebentureColumns = new DebentureColumn[6]
			{
				new DebentureColumn() { Rows = yearRows },
				new DebentureColumn() { Rows = totalValueRows },
				new DebentureColumn() { Rows = totalRateRows },
				new DebentureColumn() { Rows = interestProfitRows },
				new DebentureColumn() { Rows = interestSumRows},
				new DebentureColumn() { Rows = totalProfitRows }
			};

			debentureResult.DebentureInfo.Add(Tuple.Create("Koszt obligacji", Helper.MoneyFormat(totalValue[0])));
			debentureResult.DebentureInfo.Add(Tuple.Create("Całkowity zysk", totalProfitRows[3]));
			debentureResult.DebentureInfo.Add(Tuple.Create("Wysokość rocznego procentu składanego (długosc: 3 lata, kapitalizacja: roczna)", Helper.PercentFormat(percentageCompound * 100)));
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
					this.Head = new DebentureHead[6]
					{
						new DebentureHead() { Head = "Miesiąc", ToolTip = "Czas od wykupienia danej obligacji w miesiącach" },
						new DebentureHead() { Head = "Całkowita wartość", ToolTip = "Wartość obligacji na początku danego okresu" },
						new DebentureHead() { Head = "Oprocentowanie" },
						new DebentureHead() { Head = "Odsetki" },
						new DebentureHead() { Head = "Podatek" },
						new DebentureHead() { Head = "Zysk netto" }
					};
					break;
				case DebentureType.DOS:
					this.Head = new DebentureHead[6]
					{
						new DebentureHead() { Head = "Rok", ToolTip = "Czas od wykupienia danej obligacji w latach" },
						new DebentureHead() { Head = "Całkowita wartość", ToolTip = "Wartość obligacji na początku danego okresu" },
						new DebentureHead() { Head = "Oprocentowanie" },
						new DebentureHead() { Head = "Odsetki" },
						new DebentureHead() { Head = "Podatek" },
						new DebentureHead() { Head = "Zysk netto" }
					};
					break;
				case DebentureType.TOZ:
					this.Head = new DebentureHead[6]
					{
						new DebentureHead() { Head = "Rok", ToolTip = "Czas od wykupienia danej obligacji w latach" },
						new DebentureHead() { Head = "Całkowita wartość", ToolTip = "Wartość obligacji na początku danego okresu" },
						new DebentureHead() { Head = "Oprocentowanie" },
						new DebentureHead() { Head = "Odsetki" },
						new DebentureHead() { Head = "Suma odsetek" },
						new DebentureHead() { Head = "Zysk netto" }
					};
					break;
				case DebentureType.COI:
					this.Head = new DebentureHead[6]
					{
						new DebentureHead() { Head = "Rok", ToolTip = "Czas od wykupienia danej obligacji w latach" },
						new DebentureHead() { Head = "Całkowita wartość", ToolTip = "Wartość obligacji na początku danego okresu" },
						new DebentureHead() { Head = "Oprocentowanie" },
						new DebentureHead() { Head = "Odsetki" },
						new DebentureHead() { Head = "Suma odsetek" },
						new DebentureHead() { Head = "Zysk netto", ToolTip = "Zysk przy wypłacie pod koniec danego okresu. (Z uwzględnieniem kosztów wykupu obligacji)" }
					};
					break;
				case DebentureType.EDO:
					this.Head = new DebentureHead[5]
					{
						new DebentureHead() { Head = "Rok", ToolTip = "Czas od wykupienia danej obligacji w latach" },
						new DebentureHead() { Head = "Całkowita wartość", ToolTip = "Wartość obligacji na początku danego okresu" },
						new DebentureHead() { Head = "Oprocentowanie" },
						new DebentureHead() { Head = "Odsetki", ToolTip = "Naliczone odestki w danym okresie" },
						new DebentureHead() { Head = "Zysk netto", ToolTip = "Zysk przy wypłacie pod koniec danego okresu. (Z uwzględnieniem kosztów wykupu obligacji)"}
					};
					break;
				case DebentureType.TOS:
					this.Head = new DebentureHead[6]
					{
						new DebentureHead() { Head = "Rok", ToolTip = "Czas od wykupienia danej obligacji w latach" },
						new DebentureHead() { Head = "Całkowita wartość", ToolTip = "Wartość obligacji na początku danego okresu" },
						new DebentureHead() { Head = "Oprocentowanie" },
						new DebentureHead() { Head = "Odsetki", ToolTip = "Naliczone odestki w danym okresie" },
						new DebentureHead() { Head = "Suma odsetek" },
						new DebentureHead() { Head = "Zysk netto", ToolTip = "Zysk przy wypłacie pod koniec danego okresu. (Z uwzględnieniem kosztów wykupu obligacji)" }
					};
					break;
				default:
					break;
			}
		}
		public DebentureHead[] Head { get; set; }
		public DebentureColumn[] DebentureColumns { get; set; }
	}

	public class DebentureHead
	{
		public string ToolTip { get; set; }
		public string Head { get; set; }
	}

	public class DebentureColumn
	{
		public string[] Rows { get; set; }
	}

}
