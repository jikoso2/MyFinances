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
			var yearRows = new string[4];
			var totalValueRows = new string[4];
			var interestRateRows = new string[4];
			var interestProfitRows = new string[4];
			var taxRows = new string[4];
			var totalProfitRows = new string[4];

			var interestRate = (double) DebentureModel.DOSPercentage;
			var interestProfitAfter1Year = Math.Round(debentureResult.TotalPrice * interestRate / 100, 2);
			var interestProfitAfter2Year = Math.Round((debentureResult.TotalPrice + interestProfitAfter1Year) * interestRate / 100, 2) + interestProfitAfter1Year;
			var tax = (Math.Floor(interestProfitAfter2Year * 19) + 1) / 100 < 0.01 ? 0 : (Math.Floor(interestProfitAfter2Year * 19) + 1) / 100;

			interestProfitRows[0] = Helper.MoneyFormat(0);
			interestProfitRows[1] = Helper.MoneyFormat(interestProfitAfter1Year);
			interestProfitRows[2] = DebentureModel.BelkaTax ? Helper.MoneyFormat(interestProfitAfter2Year - tax) : Helper.MoneyFormat(interestProfitAfter2Year);

			totalProfitRows[0] = Helper.MoneyFormat(0);
			totalProfitRows[1] = Helper.MoneyFormat(interestProfitAfter1Year);
			totalProfitRows[2] = Helper.MoneyFormat(DebentureModel.BelkaTax ? interestProfitAfter2Year - tax : interestProfitAfter2Year);

			for (int i = 0; i <= 2; i++)
			{
				yearRows[i] = i.ToString();
				totalValueRows[i] = Helper.MoneyFormat(debentureResult.TotalPrice);
				interestRateRows[i] = $"{interestRate}%";
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
			debentureResult.FinalProfit = totalProfitRows[2];
		}

		private void CalculateTOZ(Debenture debentureResult)
		{
		}

		private void CalculateCOI(Debenture debentureResult)
		{
		}

		private void CalculateOTS(Debenture debentureResult)
		{
			var monthRows = new string[4];
			var totalValueRows = new string[4];
			var interestRateRows = new string[4];
			var interestProfitRows = new string[4];
			var taxRows = new string[4];
			var totalProfitRows = new string[4];

			var interestRate = (double) DebentureModel.OTSPercentage;
			var interestProfitAfter3Months = Math.Round(debentureResult.TotalPrice * interestRate / 100 / 4, 2);
			var tax = (Math.Floor(interestProfitAfter3Months * 19) + 1) / 100 < 0.01 ? 0 : (Math.Floor(interestProfitAfter3Months * 19) + 1) / 100;


			for (int i = 0; i <= 3; i++)
			{
				monthRows[i] = i.ToString();
				totalValueRows[i] = Helper.MoneyFormat(debentureResult.TotalPrice);
				interestRateRows[i] = $"{interestRate}%";
				taxRows[i] = i == 3 && DebentureModel.BelkaTax ? Helper.MoneyFormat(tax) : Helper.MoneyFormat(0);
				interestProfitRows[i] = i == 3 ? Helper.MoneyFormat(interestProfitAfter3Months) : Helper.MoneyFormat(0);
				totalProfitRows[i] = i == 3 ? Helper.MoneyFormat(DebentureModel.BelkaTax ? interestProfitAfter3Months - tax : interestProfitAfter3Months) : Helper.MoneyFormat(0);
			}

			debentureResult.DebentureData.DebentureColumns = new DebentureColumn[6]
			{
				new DebentureColumn() { Rows = monthRows },
				new DebentureColumn() { Rows = totalValueRows },
				new DebentureColumn() { Rows = interestRateRows },
				new DebentureColumn() { Rows = interestProfitRows },
				new DebentureColumn() { Rows = taxRows },
				new DebentureColumn() { Rows = totalProfitRows }
			};
			debentureResult.FinalProfit = totalProfitRows[3];
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
			this.TotalPrice = debentureModel.Amount * 100;
			this.DebentureData = new DebentureResult(debentureModel.Type);
		}
		public double TotalPrice { get; set; }
		public DebentureResult DebentureData { get; set; }
		public string FinalProfit { get; set; }
		public string InformationAboutDebenture { get; set; } = "Podstawowe Informację dotyczące typu obligacji";
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
