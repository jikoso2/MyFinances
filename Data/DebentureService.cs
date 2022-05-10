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
		}

		private void CalculateTOZ(Debenture debentureResult)
		{
		}

		private void CalculateCOI(Debenture debentureResult)
		{
		}

		private void CalculateOTS(Debenture debentureResult)
		{
			debentureResult.DebentureData = new DebentureResult();

			var monthRows = new string[4];
			var totalValueRows = new string[4];
			var interestRateRows = new string[4];
			var interestProfitRows = new string[4];
			var taxRows = new string[4];
			var totalProfitRows = new string[4];

			var interestRate = DebentureModel.OTSPercentage;
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
		}
		public double TotalPrice { get; set; }
		public DebentureResult DebentureData { get; set; }
		public string InformationAboutDebenture { get; set; } = "Podstawowe Informację dotyczące typu obligacji";
	}

	public class DebentureResult
	{
		public DebentureResult()
		{
			this.Head = new string[6] { "Miesiąc", "Całkowita wartość", "Oprocentowanie", "Odsetki", "Podatek", "Zysk netto" };
		}
		public string[] Head { get; set; }
		public DebentureColumn[] DebentureColumns { get; set; }
	}

	public class DebentureColumn
	{
		public string[] Rows { get; set; }
	}

}
