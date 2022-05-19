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
			var yearRows = Enumerable.Range(0, 3).Select(a => a.ToString()).ToArray();

			var totalValues = new int[3];
			Array.Fill(totalValues, DebentureModel.Amount * 100);
			var totalValueRows = totalValues.Select(a => Helper.MoneyFormat(a)).ToArray();

			var interestRateRows = new string[3];
			var interestProfitRows = new string[3];
			var taxRows = new string[3];
			var totalProfitRows = new string[3];

			var interestRate = DebentureModel.DOSPercentage;
			var interestProfitAfter1Year = Math.Round(DebentureModel.Amount * interestRate, 2);
			var interestProfitAfter2Year = Math.Round((DebentureModel.Amount * 100 + interestProfitAfter1Year) * interestRate / 100, 2) + interestProfitAfter1Year;
			var tax = (Math.Floor(interestProfitAfter2Year * 19) + 1) / 100 < 0.01 ? 0 : (Math.Floor(interestProfitAfter2Year * 19) + 1) / 100;

			interestProfitRows[0] = Helper.MoneyFormat(0);
			interestProfitRows[1] = Helper.MoneyFormat(interestProfitAfter1Year);
			interestProfitRows[2] = DebentureModel.BelkaTax ? Helper.MoneyFormat(interestProfitAfter2Year - tax) : Helper.MoneyFormat(interestProfitAfter2Year);

			totalProfitRows[0] = Helper.MoneyFormat(0);
			totalProfitRows[1] = Helper.MoneyFormat(interestProfitAfter1Year);
			totalProfitRows[2] = Helper.MoneyFormat(DebentureModel.BelkaTax ? interestProfitAfter2Year - tax : interestProfitAfter2Year);

			for (int i = 0; i <= 2; i++)
			{
				interestRateRows[i] = Helper.PercentFormat(interestRate);
				taxRows[i] = i == 2 && DebentureModel.BelkaTax ? Helper.MoneyFormat(tax) : Helper.MoneyFormat(0);
			}

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
		}

		private void CalculateCOI(Debenture debentureResult)
		{
		}

		private void CalculateOTS(Debenture debentureResult)
		{
			var totalValue = new double[4];
			Array.Fill(totalValue, DebentureModel.Amount*100);
			var interestRate = new double[4];
			Array.Fill(interestRate, DebentureModel.OTSPercentage);
			var interestProfit = new double[4];
			var tax = new double[4];
			var totalProfit = new double[4];


			for (int i = 0; i <= 3; i++)
			{
				var profit = Math.Round(DebentureModel.Amount * interestRate[i] * ((double)i/12), 2);
				var calculatedTax = (Math.Floor(profit * 19) + 1) / 100 < 0.01 ? 0 : (Math.Floor(profit * 19) + 1) / 100;
				
				tax[i] = i == 3 && DebentureModel.BelkaTax ? calculatedTax : 0;
				interestProfit[i] = i == 3 ? profit : 0;
				totalProfit[i] = i == 3 ? profit-tax[i]: 0;
			}

			var interestRateRows = interestRate.Select(a => Helper.PercentFormat(a)).ToArray();
			var monthRows = Enumerable.Range(0, 4).Select(a => a.ToString()).ToArray();
			var totalValueRows = totalValue.Select(a => Helper.MoneyFormat(a)).ToArray();
			var interestProfitRows = interestProfit.Select(a=>Helper.MoneyFormat(a)).ToArray();
			var taxRows = tax.Select(a=>Helper.MoneyFormat(a)).ToArray();
			var totalProfitRows = totalProfit.Select(a=>Helper.MoneyFormat(a)).ToArray();

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
