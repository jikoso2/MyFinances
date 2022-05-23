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
			var interestRate = DebentureModel.TOZPercentage.Select(a => a / 100).ToList();
			interestRate.Insert(0, 0);
			var interestProfit = new double[7];
			var tax = new double[7];
			var totalProfit = new double[7];

			for (int i = 0; i < 7; i++)
			{
				var profit = Math.Round(totalValue[i] * interestRate[i] / 2, 2);

				if (i < 6)
					totalValue[i + 1] = totalValue[i] + profit;

				interestProfit[i] = profit;

				if (i > 0)
					totalProfit[i] = totalProfit[i - 1] + profit;

				var calculatedTax = (Math.Floor(totalProfit[i] * 19) + 1) / 100;

				tax[i] = i == 6 && DebentureModel.BelkaTax ? calculatedTax : 0;

				if (i == 6 && DebentureModel.BelkaTax)
					totalProfit[i] -= calculatedTax;

			}

			var interestRateRows = interestRate.Select(a => Helper.PercentFormat(a)).ToArray();
			var yearRows = Enumerable.Range(0, 7).Select(a => (Math.Round((double)a / 2, 1)).ToString()).ToArray();
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
					this.Head = new string[6] { "Rok", "Całkowita wartość", "Oprocentowanie", "Odsetki", "Podatek", "Zysk netto" };
					break;
				case DebentureType.COI:
					this.Head = new string[6] { "Rok", "Całkowita wartość", "Oprocentowanie", "Odsetki", "Podatek", "Zysk netto" };
					break;
				case DebentureType.EDO:
					this.Head = new string[6] { "Rok", "Całkowita wartość", "Oprocentowanie", "Odsetki", "Podatek", "Zysk netto" };
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
