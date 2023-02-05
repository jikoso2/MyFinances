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

			var oneDebentureVal = 100.0;

			var firstProfitPerDebenture = Math.Round(oneDebentureVal * DebentureModel.DOSPercentage / 100, 2);
			var firstTaxPerDebenture = (Math.Floor(firstProfitPerDebenture * 19) + 1) / 100;

			tax[0] = DebentureModel.BelkaTax ? firstTaxPerDebenture * DebentureModel.Amount : 0;
			interestProfit[0] = firstProfitPerDebenture * DebentureModel.Amount;
			totalProfit[1] = (firstProfitPerDebenture - 0.7) * DebentureModel.Amount - tax[0];
			totalValue[1] += firstProfitPerDebenture * DebentureModel.Amount;

			var secondProfitPerDebenture = Math.Round((oneDebentureVal + firstProfitPerDebenture) * DebentureModel.DOSPercentage / 100, 2);
			var secondTaxPerDebenture = (Math.Floor(secondProfitPerDebenture * 19) + 1) / 100;

			tax[1] = DebentureModel.BelkaTax ? secondTaxPerDebenture * DebentureModel.Amount : 0;
			totalValue[2] = totalValue[1] + secondProfitPerDebenture * DebentureModel.Amount;
			interestProfit[1] = secondProfitPerDebenture * DebentureModel.Amount;
			totalProfit[2] = DebentureModel.BelkaTax ? Math.Floor((secondProfitPerDebenture + firstProfitPerDebenture) * 81) / 100 : secondProfitPerDebenture + firstProfitPerDebenture;
			totalProfit[2] *= DebentureModel.Amount;

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

			debentureResult.DebentureInfo.Add(Tuple.Create("Całkowity zysk", totalProfitRows[2]));
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
				double profit = i <= 6 ? Math.Round(totalValue[i] / DebentureModel.Amount * interestRate[i] / 2, 2) * DebentureModel.Amount : 0;

				interestProfit[i] = profit;

				//interestSum[i + 1] = i > 0 ? interestSum[i - 1] + profit : profit;

				if (i < 6)
				{
					totalProfit[i + 1] = i == 0 ? profit : totalProfit[i] + profit;

					if (DebentureModel.BelkaTax)
					{
						//var sumPerDebenture = interestProfit.ToList().Where(a => a > 0).Select(a => Math.Ceiling((a - 0.7) / DebentureModel.Amount * 19) / 100).Sum();
						var sumPerDebenture = Math.Ceiling((totalProfit[i + 1] / DebentureModel.Amount - 0.7) * 19) / 100;
						if (i == 5)
							sumPerDebenture = Math.Ceiling((totalProfit[i + 1] / DebentureModel.Amount) * 19) / 100;

						calculatedTax = Math.Max(sumPerDebenture * DebentureModel.Amount, 0);
					}
				}

				if (i < 5)
					totalProfitRes[i + 1] = Math.Max(totalProfit[i + 1] - (0.7 * DebentureModel.Amount), 0);
				else if (i < 6)
					totalProfitRes[i + 1] = totalProfit[i + 1];

				if (i < 6)
					totalProfitRes[i + 1] -= calculatedTax;

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
				if (i >= 1 && i < 4)
					interestRate[i] = interestRate[i] <= 0 ? DebentureModel.COIAdditionalPercentage / 100 : DebentureModel.COIAdditionalPercentage / 100 + interestRate[i];

				double profitPerDebenture = i <= 4 ? Math.Round(totalValue[i] / DebentureModel.Amount * interestRate[i], 2) : 0;

				interestProfit[i] = profitPerDebenture * DebentureModel.Amount;

				if (DebentureModel.BelkaTax)
					calculatedTax = (Math.Ceiling(profitPerDebenture * 19) * DebentureModel.Amount) / 100;

				if (i < 4)
					totalProfit[i + 1] = profitPerDebenture * DebentureModel.Amount - calculatedTax + totalProfit[i];

				totalProfitRes[i] = totalProfit[i] - 0.7 * DebentureModel.Amount > 0 && i < 4 ? totalProfit[i] - 0.7 * DebentureModel.Amount : totalProfit[i];
			}

			var interestRateRows = interestRate.Select(a => Helper.PercentFormat(a * 100)).ToArray();
			var yearRows = Enumerable.Range(0, 5).Select(a => a.ToString()).ToArray();
			var totalValueRows = totalValue.Select(a => Helper.MoneyFormat(a)).ToArray();
			var interestProfitRows = interestProfit.Select(a => Helper.MoneyFormat(a)).ToArray();
			var interestSumRows = totalProfit.Select(a => Helper.MoneyFormat(a)).ToArray();
			var totalProfitRows = totalProfitRes.Select(a => Helper.MoneyFormat(a)).ToArray();

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
			var sumInterestProfit = new double[11];
			var tax = new double[11];
			var totalProfit = new double[11];
			var totalProfitRes = new double[11];
			double calculatedTax = 0.0;
			double earlyRedemptionFee = 2.0;

			for (int i = 0; i < 11; i++)
			{
				if (i >= 1 && i < 10)
					interestRate[i] = interestRate[i] <= 0 ? DebentureModel.EDOAdditionalPercentage / 100 : DebentureModel.EDOAdditionalPercentage / 100 + interestRate[i];
				else
					interestRate[i] = interestRate[i];

				double profitPerDebenture = i <= 10 ? Math.Round(totalValue[i] / DebentureModel.Amount * interestRate[i], 2) : 0;

				if (i < 10)
					totalValue[i + 1] = totalValue[i] + profitPerDebenture * DebentureModel.Amount;

				interestProfit[i] = profitPerDebenture * DebentureModel.Amount;

				if (i > 0)
					sumInterestProfit[i] = interestProfit[i] + sumInterestProfit[i - 1];
				else
					sumInterestProfit[i] = interestProfit[i];

				if (i < 10)
					totalProfit[i + 1] = i == 0 ? profitPerDebenture * DebentureModel.Amount : totalProfit[i] + profitPerDebenture * DebentureModel.Amount;

				if (DebentureModel.BelkaTax)
				{
					calculatedTax = (Math.Ceiling(Math.Max(totalProfit[i] / DebentureModel.Amount - earlyRedemptionFee, 0) * 19)) / 100 * DebentureModel.Amount;
					if (i == 10)
						calculatedTax = Math.Ceiling(totalProfit[i] / DebentureModel.Amount * 19) / 100 * DebentureModel.Amount;
				}

				totalProfitRes[i] = i < 10 ? totalProfit[i] - calculatedTax - earlyRedemptionFee * DebentureModel.Amount: totalProfit[i] - calculatedTax;
				if (i < 10)
					totalProfitRes[i] = totalProfitRes[i] > 0 ? totalProfitRes[i] : 0;

			}

			var yearRows = Enumerable.Range(0, 11).Select(a => a.ToString()).ToArray();
			var interestRateRows = interestRate.Select(a => Helper.PercentFormat(a * 100)).ToArray();
			var totalValueRows = totalValue.Select(a => Helper.MoneyFormat(a)).ToArray();
			var interestProfitRows = interestProfit.Select(a => Helper.MoneyFormat(a)).ToArray();
			var sumInterestProfitRows = sumInterestProfit.Select(a => Helper.MoneyFormat(a)).ToArray();
			var totalProfitRows = totalProfitRes.Select(a => Helper.MoneyFormat(a)).ToArray();

			debentureResult.DebentureData.DebentureColumns = new DebentureColumn[6]
			{
				new DebentureColumn() { Rows = yearRows },
				new DebentureColumn() { Rows = totalValueRows },
				new DebentureColumn() { Rows = interestRateRows },
				new DebentureColumn() { Rows = interestProfitRows },
				new DebentureColumn() { Rows = sumInterestProfitRows },
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

			debentureResult.DebentureInfo.Add(Tuple.Create("Miesięczny zysk", Helper.MoneyFormat(monthlyProfit * DebentureModel.Amount)));
			debentureResult.DebentureInfo.Add(Tuple.Create($"Całkowity zysk ({totalLength} msc)", Helper.MoneyFormat(monthlyProfit * totalLength * DebentureModel.Amount)));

			if (DebentureModel.BelkaTax)
			{
				debentureResult.DebentureInfo.Add(Tuple.Create("Miesięczny podatek", Helper.MoneyFormat(monthlyTaxPerDebenture * DebentureModel.Amount)));
				debentureResult.DebentureInfo.Add(Tuple.Create($"Całkowity podatek ({totalLength} msc)", Helper.MoneyFormat(monthlyTaxPerDebenture * totalLength * DebentureModel.Amount)));
				debentureResult.DebentureInfo.Add(Tuple.Create("Rzeczywiste oprocentowanie po odprowadzeniu podatku", Helper.PercentFormat(realPercentage)));
			}
			else
			{
				debentureResult.DebentureInfo.Add(Tuple.Create("Rzeczywiste oprocentowanie", Helper.PercentFormat(realPercentage)));
			}
		}

		private void CalculateTOS(Debenture debentureResult)
		{
			var percentage = DebentureModel.TOSPercentage / 100;
			var percentageCompound = Math.Round(((Math.Pow((1 + percentage), 3) - 1)) / 3, 4);

			var totalValue = new double[4];
			Array.Fill(totalValue, DebentureModel.Amount * 100);
			var interestRates = new double[4];
			Array.Fill(interestRates, DebentureModel.TOSPercentage / 100);

			var interestProfit = new double[4];
			var totalProfit = new double[4];
			var totalProfitRes = new double[4];
			var interestSum = new double[4];

			double calculatedTax = 0;

			for (int i = 0; i < 4; i++)
			{
				double profit = i <= 3 ? Math.Round((totalValue[i] / DebentureModel.Amount) * interestRates[i], 2) * DebentureModel.Amount : 0;

				if (i < 3)
					interestProfit[i] = profit;
				else
					interestProfit[i] = 0;

				if (i < 3)
					totalProfit[i + 1] = i == 0 ? profit : totalProfit[i] + profit;

				if (DebentureModel.BelkaTax)
				{
					calculatedTax = (Math.Ceiling(Math.Max(totalProfit[i] / DebentureModel.Amount - 0.70, 0) * 19)) / 100 * DebentureModel.Amount;
					if (i == 3)
						calculatedTax = Math.Ceiling(totalProfit[i] / DebentureModel.Amount * 19) / 100 * DebentureModel.Amount;
				}

				totalProfitRes[i] = totalProfit[i] - calculatedTax;
				totalProfitRes[i] = i != 3 && i != 0 ? totalProfitRes[i] - 0.70 * DebentureModel.Amount : totalProfitRes[i];

				if (i < 3)
					totalValue[i + 1] = totalValue[i] + profit;
			}

			var yearRows = Enumerable.Range(0, 4).Select(a => (Math.Round((double)a, 1)).ToString()).ToArray();
			var totalValueRows = totalValue.Select(a => Helper.MoneyFormat(a)).ToArray();
			var totalRateRows = interestRates.Select(a => Helper.PercentFormat(a * 100)).ToArray();
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
						new DebentureHead() { Head = "Odsetki", ToolTip = "Wysokość odsetek przed potrąceniem podatku"},
						new DebentureHead() { Head = "Suma odsetek", ToolTip = "Suma odsetek po potrąceniem podatku" },
						new DebentureHead() { Head = "Zysk netto", ToolTip = "Zysk przy wypłacie pod koniec danego okresu. (Z uwzględnieniem kosztów wykupu obligacji)" }
					};
					break;
				case DebentureType.EDO:
					this.Head = new DebentureHead[6]
					{
						new DebentureHead() { Head = "Rok", ToolTip = "Czas od wykupienia danej obligacji w latach" },
						new DebentureHead() { Head = "Całkowita wartość", ToolTip = "Wartość obligacji na początku danego okresu" },
						new DebentureHead() { Head = "Oprocentowanie" },
						new DebentureHead() { Head = "Odsetki", ToolTip = "Naliczone odestki w danym okresie" },
						new DebentureHead() { Head = "Suma odsetek", ToolTip = "Zsumowane odsetki" },
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
