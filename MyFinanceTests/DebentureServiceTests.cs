using Bunit;
using MyFinances.Pages;
using MyFinances.Data;
using MyFinances.Models;
using Microsoft.Extensions.DependencyInjection;

namespace MyFinanceTests
{
	public class DebentureServiceTests
	{
		[Fact]
		public void RenderServiceTest()
		{
			var ctx = new TestContext();
			ctx.Services.AddSingleton(new DebentureService());
			var cut = ctx.RenderComponent<DebenturesCalculator>();

			Assert.Equal(" Kwota inwestycji: 100.00 zł", cut.Find("[id=\"AmountCalc\"]").TextContent);

			foreach (DebentureType type in (DebentureType[])Enum.GetValues(typeof(DebentureType)))
			{
				var debentureTypeInput = cut.Find("select");
				debentureTypeInput.Change(type);
				var allButtons = cut.FindAll("button");

				var infoButton = allButtons.Where(x => x.TextContent.Contains($"Informacje dotyczące obligacji {type}")).FirstOrDefault();
				Assert.NotNull(infoButton);

				var calculateButton = allButtons.Where(x => x.TextContent.Equals("Oblicz")).FirstOrDefault();
				Assert.NotNull(calculateButton);

				var inputs = cut.FindAll("input");
				switch (type)
				{
					case DebentureType.OTS:
						Assert.Equal(3, inputs.Count);
						break;
					case DebentureType.DOS:
						Assert.Equal(3, inputs.Count);
						break;
					case DebentureType.TOZ:
						Assert.Equal(8, inputs.Count);
						break;
					case DebentureType.COI:
						Assert.Equal(6, inputs.Count);
						break;
					case DebentureType.EDO:
						Assert.Equal(14, inputs.Count);
						break;
					case DebentureType.ROR:
						Assert.Equal(4, inputs.Count);
						break;
					case DebentureType.DOR:
						Assert.Equal(4, inputs.Count);
						break;
					case DebentureType.TOS:
						Assert.Equal(4, inputs.Count);
						break;
					default:
						break;
				}
			}
		}

		[Fact]
		public void OTSServiceTest()
		{
			var service = new DebentureService();
			var testModel = new DebentureModel() { Type = DebentureType.OTS };

			var result = service.GetDebentureAsync(testModel).Result;

			Assert.NotNull(result);
			Assert.Equal(6, result.DebentureData.DebentureColumns.Length);
			Assert.Equal(6, result.DebentureData.Head.Length);
			Assert.Single(result.DebentureInfo);

			testModel.Type = DebentureType.OTS;
			testModel.OTSPercentage = 3;
			testModel.Amount = 85;
			testModel.BelkaTax = false;
			result = service.GetDebentureAsync(testModel).Result;
			var expected = new Tuple<string, string>("Całkowity zysk", "63.75 zł");
			Assert.Equal(expected, result.DebentureInfo.First());

			testModel.OTSPercentage = 3;
			testModel.Amount = 85;
			testModel.BelkaTax = true;
			result = service.GetDebentureAsync(testModel).Result;
			expected = new Tuple<string, string>("Całkowity zysk", "51.00 zł");
			Assert.Equal(expected, result.DebentureInfo.First());

			testModel.OTSPercentage = 3.5;
			testModel.Amount = 33;
			testModel.BelkaTax = false;
			result = service.GetDebentureAsync(testModel).Result;
			expected = new Tuple<string, string>("Całkowity zysk", "29.04 zł");
			Assert.Equal(expected, result.DebentureInfo.First());

			testModel.OTSPercentage = 3.5;
			testModel.Amount = 33;
			testModel.BelkaTax = true;
			result = service.GetDebentureAsync(testModel).Result;
			expected = new Tuple<string, string>("Całkowity zysk", "23.43 zł");
			Assert.Equal(expected, result.DebentureInfo.First());
		}

		[Fact]
		public void OTSServiceRenderTest()
		{
			var ctx = new TestContext();
			ctx.Services.AddSingleton(new DebentureService());
			var cut = ctx.RenderComponent<DebenturesCalculator>();

			var debentureTypeInput = cut.Find("select");
			debentureTypeInput.Change(DebentureType.OTS);
			var allButtons = cut.FindAll("button");

			var calculateButton = allButtons.Where(x => x.TextContent.Equals("Oblicz")).FirstOrDefault();
			Assert.NotNull(calculateButton);
			var inputs = cut.FindAll("input");

			var amountInput = inputs[0];
			var percentageInput = inputs[1];

			amountInput.Change(100);
			percentageInput.Input(3.25);
			calculateButton.Click();

			var output = cut.FindAll("th");
			Assert.Equal("Miesiąc", output[0].TextContent);
			Assert.Equal("Całkowita wartość", output[1].TextContent);
			Assert.Equal("Oprocentowanie", output[2].TextContent);
			Assert.Equal("Odsetki", output[3].TextContent);
			Assert.Equal("Podatek", output[4].TextContent);
			Assert.Equal("Zysk netto", output[5].TextContent);

			Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Całkowity zysk")).Any());
		}

		[Fact]
		public void DOSServiceTest()
		{
			var service = new DebentureService();
			var testModel = new DebentureModel() { Type = DebentureType.DOS };

			var result = service.GetDebentureAsync(testModel).Result;

			Assert.NotNull(result);
			Assert.Equal(6, result.DebentureData.DebentureColumns.Length);
			Assert.Equal(6, result.DebentureData.Head.Length);
			Assert.Single(result.DebentureInfo);

			testModel.DOSPercentage = 6;
			testModel.Amount = 85;
			testModel.BelkaTax = false;
			result = service.GetDebentureAsync(testModel).Result;
			var expected = new Tuple<string, string>("Całkowity zysk", "1 050.60 zł");
			Assert.Equal(expected, result.DebentureInfo.First());

			testModel.DOSPercentage = 6;
			testModel.Amount = 85;
			testModel.BelkaTax = true;
			result = service.GetDebentureAsync(testModel).Result;
			expected = new Tuple<string, string>("Całkowity zysk", "850.85 zł");
			Assert.Equal(expected, result.DebentureInfo.First());

			testModel.DOSPercentage = 4.5;
			testModel.Amount = 33;
			testModel.BelkaTax = false;
			result = service.GetDebentureAsync(testModel).Result;
			expected = new Tuple<string, string>("Całkowity zysk", "303.60 zł");
			Assert.Equal(expected, result.DebentureInfo.First());

			testModel.DOSPercentage = 4.5;
			testModel.Amount = 33;
			testModel.BelkaTax = true;
			result = service.GetDebentureAsync(testModel).Result;
			expected = new Tuple<string, string>("Całkowity zysk", "245.85 zł");
			Assert.Equal(expected, result.DebentureInfo.First());
		}

		[Fact]
		public void DOSServiceRenderTest()
		{
			var ctx = new TestContext();
			ctx.Services.AddSingleton(new DebentureService());
			var cut = ctx.RenderComponent<DebenturesCalculator>();

			var debentureTypeInput = cut.Find("select");
			debentureTypeInput.Change(DebentureType.DOS);
			var allButtons = cut.FindAll("button");

			var calculateButton = allButtons.Where(x => x.TextContent.Equals("Oblicz")).FirstOrDefault();
			Assert.NotNull(calculateButton);
			var inputs = cut.FindAll("input");

			var amountInput = inputs[0];
			var percentageInput = inputs[1];

			amountInput.Change(100);
			percentageInput.Input(3.25);
			calculateButton.Click();

			var output = cut.FindAll("th");
			Assert.Equal("Rok", output[0].TextContent);
			Assert.Equal("Całkowita wartość", output[1].TextContent);
			Assert.Equal("Oprocentowanie", output[2].TextContent);
			Assert.Equal("Odsetki", output[3].TextContent);
			Assert.Equal("Podatek", output[4].TextContent);
			Assert.Equal("Zysk netto", output[5].TextContent);

			Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Całkowity zysk")).Any());
		}

		[Fact]
		public void TOZServiceTest()
		{
			var service = new DebentureService();
			var testModel = new DebentureModel() { Type = DebentureType.TOZ };

			var result = service.GetDebentureAsync(testModel).Result;

			Assert.NotNull(result);
			Assert.Equal(6, result.DebentureData.DebentureColumns.Length);
			Assert.Equal(6, result.DebentureData.Head.Length);
			Assert.Single(result.DebentureInfo);

			testModel.TOZPercentage = new List<double>() { 6, 6, 6, 6, 6, 6 };
			testModel.Amount = 85;
			testModel.BelkaTax = false;
			var expected = new Tuple<string, string>("Całkowity zysk", "1 530.00 zł");
			result = service.GetDebentureAsync(testModel).Result;

			Assert.Equal(expected, result.DebentureInfo.First());

			testModel.TOZPercentage = new List<double>() { 6, 6, 6, 6, 6, 6 };
			testModel.Amount = 85;
			testModel.BelkaTax = true;
			result = service.GetDebentureAsync(testModel).Result;
			expected = new Tuple<string, string>("Całkowity zysk", "1 239.30 zł");
			Assert.Equal(expected, result.DebentureInfo.First());

			testModel.TOZPercentage = new List<double>() { 3, 4.5, 6, 5.5, 1.08, 2 };
			testModel.Amount = 33;
			testModel.BelkaTax = false;
			result = service.GetDebentureAsync(testModel).Result;
			expected = new Tuple<string, string>("Całkowity zysk", "364.32 zł");
			Assert.Equal(expected, result.DebentureInfo.First());

			testModel.TOZPercentage = new List<double>() { 3, 4.5, 6, 5.5, 1.08, 2 };
			testModel.Amount = 33;
			testModel.BelkaTax = true;
			result = service.GetDebentureAsync(testModel).Result;
			expected = new Tuple<string, string>("Całkowity zysk", "295.02 zł");
			Assert.Equal(expected, result.DebentureInfo.First());
		}

		[Fact]
		public void TOZServiceRenderTest()
		{
			var ctx = new TestContext();
			ctx.Services.AddSingleton(new DebentureService());
			var cut = ctx.RenderComponent<DebenturesCalculator>();

			var debentureTypeInput = cut.Find("select");
			debentureTypeInput.Change(DebentureType.TOZ);
			var allButtons = cut.FindAll("button");

			var calculateButton = allButtons.Where(x => x.TextContent.Equals("Oblicz")).FirstOrDefault();
			Assert.NotNull(calculateButton);
			var inputs = cut.FindAll("input");

			var amountInput = inputs[0];
			var percentageInput1 = inputs[1];
			cut.FindAll("input");
			var percentageInput2 = inputs[2];
			var percentageInput3 = inputs[3];
			var percentageInput4 = inputs[4];
			var percentageInput5 = inputs[5];
			var percentageInput6 = inputs[6];

			amountInput.Change(100);
			calculateButton.Click();

			var output = cut.FindAll("th");
			Assert.Equal("Rok", output[0].TextContent);
			Assert.Equal("Całkowita wartość", output[1].TextContent);
			Assert.Equal("Oprocentowanie", output[2].TextContent);
			Assert.Equal("Odsetki", output[3].TextContent);
			Assert.Equal("Suma odsetek", output[4].TextContent);
			Assert.Equal("Zysk netto", output[5].TextContent);

			Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Całkowity zysk")).Any());
		}

		[Fact]
		public void RORServiceTest()
		{
			var service = new DebentureService();
			var testModel = new DebentureModel() { Type = DebentureType.ROR };

			var result = service.GetDebentureAsync(testModel).Result;
			Assert.NotNull(result);

			testModel.RORPercentage = 5.0;
			testModel.Amount = 85;
			testModel.BelkaTax = false;
			result = service.GetDebentureAsync(testModel).Result;
			Assert.Equal(3, result.DebentureInfo.Count);
			Assert.Equal(new Tuple<string, string>("Miesięczny zysk", "35.70 zł"), result.DebentureInfo[0]);
			Assert.Equal(new Tuple<string, string>("Całkowity zysk (12 msc)", "428.40 zł"), result.DebentureInfo[1]);
			Assert.Equal(new Tuple<string, string>("Rzeczywiste oprocentowanie", "5.04 %"), result.DebentureInfo[2]);

			testModel.RORPercentage = 5.0;
			testModel.Amount = 85;
			testModel.BelkaTax = true;
			result = service.GetDebentureAsync(testModel).Result;
			Assert.Equal(5, result.DebentureInfo.Count);
			Assert.Equal(new Tuple<string, string>("Miesięczny zysk", "28.90 zł"), result.DebentureInfo[0]);
			Assert.Equal(new Tuple<string, string>("Całkowity zysk (12 msc)", "346.80 zł"), result.DebentureInfo[1]);
			Assert.Equal(new Tuple<string, string>("Miesięczny podatek", "6.80 zł"), result.DebentureInfo[2]);
			Assert.Equal(new Tuple<string, string>("Całkowity podatek (12 msc)", "81.60 zł"), result.DebentureInfo[3]);
			Assert.Equal(new Tuple<string, string>("Rzeczywiste oprocentowanie po odprowadzeniu podatku", "4.08 %"), result.DebentureInfo[4]);

			testModel.RORPercentage = 6.70;
			testModel.Amount = 33;
			testModel.BelkaTax = false;
			result = service.GetDebentureAsync(testModel).Result;
			Assert.Equal(3, result.DebentureInfo.Count);
			Assert.Equal(new Tuple<string, string>("Miesięczny zysk", "18.48 zł"), result.DebentureInfo[0]);
			Assert.Equal(new Tuple<string, string>("Całkowity zysk (12 msc)", "221.76 zł"), result.DebentureInfo[1]);
			Assert.Equal(new Tuple<string, string>("Rzeczywiste oprocentowanie", "6.72 %"), result.DebentureInfo[2]);

			testModel.RORPercentage = 6.70;
			testModel.Amount = 33;
			testModel.BelkaTax = true;
			result = service.GetDebentureAsync(testModel).Result;
			Assert.Equal(5, result.DebentureInfo.Count);
			Assert.Equal(new Tuple<string, string>("Miesięczny zysk", "14.85 zł"), result.DebentureInfo[0]);
			Assert.Equal(new Tuple<string, string>("Całkowity zysk (12 msc)", "178.20 zł"), result.DebentureInfo[1]);
			Assert.Equal(new Tuple<string, string>("Miesięczny podatek", "3.63 zł"), result.DebentureInfo[2]);
			Assert.Equal(new Tuple<string, string>("Całkowity podatek (12 msc)", "43.56 zł"), result.DebentureInfo[3]);
			Assert.Equal(new Tuple<string, string>("Rzeczywiste oprocentowanie po odprowadzeniu podatku", "5.4 %"), result.DebentureInfo[4]);
		}

		[Fact]
		public void RORServiceRenderTest()
		{
			var ctx = new TestContext();
			ctx.Services.AddSingleton(new DebentureService());
			var cut = ctx.RenderComponent<DebenturesCalculator>();

			var debentureTypeInput = cut.Find("select");
			debentureTypeInput.Change(DebentureType.ROR);
			var allButtons = cut.FindAll("button");

			var calculateButton = allButtons.Where(x => x.TextContent.Equals("Oblicz")).FirstOrDefault();
			Assert.NotNull(calculateButton);
			var inputs = cut.FindAll("input");
			Assert.Equal(4, inputs.Count);

			var belkaTaxInput = inputs[1];
			var amountInput = inputs[2];
			var percentageInput = inputs[3];

			belkaTaxInput.Input(true);
			amountInput.Change(100);
			calculateButton.Click();

			Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Miesięczny zysk")).Any());
			Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Całkowity zysk (12 msc)")).Any());
			Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Miesięczny podatek")).Any());
			Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Całkowity podatek (12 msc)")).Any());
			Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Rzeczywiste oprocentowanie po odprowadzeniu podatku")).Any());

		}

		[Fact]
		public void DORServiceTest()
		{
			var service = new DebentureService();
			var testModel = new DebentureModel() { Type = DebentureType.DOR };

			var result = service.GetDebentureAsync(testModel).Result;
			Assert.NotNull(result);

			testModel.DORPercentage = 5.0;
			testModel.Amount = 85;
			testModel.BelkaTax = false;
			result = service.GetDebentureAsync(testModel).Result;
			Assert.Equal(3, result.DebentureInfo.Count);
			Assert.Equal(new Tuple<string, string>("Miesięczny zysk", "35.70 zł"), result.DebentureInfo[0]);
			Assert.Equal(new Tuple<string, string>("Całkowity zysk (24 msc)", "856.80 zł"), result.DebentureInfo[1]);
			Assert.Equal(new Tuple<string, string>("Rzeczywiste oprocentowanie", "5.04 %"), result.DebentureInfo[2]);

			testModel.DORPercentage = 5.0;
			testModel.Amount = 85;
			testModel.BelkaTax = true;
			result = service.GetDebentureAsync(testModel).Result;
			Assert.Equal(5, result.DebentureInfo.Count);
			Assert.Equal(new Tuple<string, string>("Miesięczny zysk", "28.90 zł"), result.DebentureInfo[0]);
			Assert.Equal(new Tuple<string, string>("Całkowity zysk (24 msc)", "693.60 zł"), result.DebentureInfo[1]);
			Assert.Equal(new Tuple<string, string>("Miesięczny podatek", "6.80 zł"), result.DebentureInfo[2]);
			Assert.Equal(new Tuple<string, string>("Całkowity podatek (24 msc)", "163.20 zł"), result.DebentureInfo[3]);
			Assert.Equal(new Tuple<string, string>("Rzeczywiste oprocentowanie po odprowadzeniu podatku", "4.08 %"), result.DebentureInfo[4]);

			testModel.DORPercentage = 6.70;
			testModel.Amount = 33;
			testModel.BelkaTax = false;
			result = service.GetDebentureAsync(testModel).Result;
			Assert.Equal(3, result.DebentureInfo.Count);
			Assert.Equal(new Tuple<string, string>("Miesięczny zysk", "18.48 zł"), result.DebentureInfo[0]);
			Assert.Equal(new Tuple<string, string>("Całkowity zysk (24 msc)", "443.52 zł"), result.DebentureInfo[1]);
			Assert.Equal(new Tuple<string, string>("Rzeczywiste oprocentowanie", "6.72 %"), result.DebentureInfo[2]);

			testModel.DORPercentage = 6.70;
			testModel.Amount = 33;
			testModel.BelkaTax = true;
			result = service.GetDebentureAsync(testModel).Result;
			Assert.Equal(5, result.DebentureInfo.Count);
			Assert.Equal(new Tuple<string, string>("Miesięczny zysk", "14.85 zł"), result.DebentureInfo[0]);
			Assert.Equal(new Tuple<string, string>("Całkowity zysk (24 msc)", "356.40 zł"), result.DebentureInfo[1]);
			Assert.Equal(new Tuple<string, string>("Miesięczny podatek", "3.63 zł"), result.DebentureInfo[2]);
			Assert.Equal(new Tuple<string, string>("Całkowity podatek (24 msc)", "87.12 zł"), result.DebentureInfo[3]);
			Assert.Equal(new Tuple<string, string>("Rzeczywiste oprocentowanie po odprowadzeniu podatku", "5.4 %"), result.DebentureInfo[4]);
		}

		[Fact]
		public void DORServiceRenderTest()
		{
			var ctx = new TestContext();
			ctx.Services.AddSingleton(new DebentureService());
			var cut = ctx.RenderComponent<DebenturesCalculator>();

			var debentureTypeInput = cut.Find("select");
			debentureTypeInput.Change(DebentureType.DOR);
			var allButtons = cut.FindAll("button");

			var calculateButton = allButtons.Where(x => x.TextContent.Equals("Oblicz")).FirstOrDefault();
			Assert.NotNull(calculateButton);
			var inputs = cut.FindAll("input");
			Assert.Equal(4, inputs.Count);

			if (inputs.Any() && calculateButton != null)
			{
				var belkaTaxInput = inputs[1];
				var amountInput = inputs[2];
				var percentageInput = inputs[3];

				belkaTaxInput.Input(true);
				amountInput.Change(100);
				calculateButton.Click();

				Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Miesięczny zysk")).Any());
				Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Całkowity zysk (24 msc)")).Any());
				Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Miesięczny podatek")).Any());
				Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Całkowity podatek (24 msc)")).Any());
				Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Rzeczywiste oprocentowanie po odprowadzeniu podatku")).Any());
			}
		}

		[Fact]
		public void TOSServiceTest()
		{
			var service = new DebentureService();
			var testModel = new DebentureModel() { Type = DebentureType.TOS };

			var result = service.GetDebentureAsync(testModel).Result;
			Assert.NotNull(result);

			testModel.TOSPercentage = 5.0;
			testModel.Amount = 85;
			testModel.BelkaTax = false;
			result = service.GetDebentureAsync(testModel).Result;
			Assert.Equal(2, result.DebentureInfo.Count);
			Assert.Equal(new Tuple<string, string>("Całkowity zysk", "1 339.60 zł"), result.DebentureInfo[0]);
			Assert.Equal(new Tuple<string, string>("Wysokość rocznego procentu składanego (długosc: 3 lata, kapitalizacja: roczna)", "5.25 %"), result.DebentureInfo[1]);
			Assert.Equal(new[] { "Rok", "Całkowita wartość", "Oprocentowanie", "Odsetki", "Suma odsetek", "Zysk netto" }, result.DebentureData.Head.Select(a => a.Head));
			Assert.Equal(new[] { "0", "1", "2", "3" }, result.DebentureData.DebentureColumns[0].Rows);
			Assert.Equal(new[] { "8 500.00 zł", "8 925.00 zł", "9 371.25 zł", "9 839.60 zł" }, result.DebentureData.DebentureColumns[1].Rows);
			Assert.Equal(new[] { "5 %", "5 %", "5 %", "5 %" }, result.DebentureData.DebentureColumns[2].Rows);
			Assert.Equal(new[] { "425.00 zł", "446.25 zł", "468.35 zł", "0.00 zł" }, result.DebentureData.DebentureColumns[3].Rows);
			Assert.Equal(new[] { "0.00 zł", "425.00 zł", "871.25 zł", "1 339.60 zł" }, result.DebentureData.DebentureColumns[4].Rows);
			Assert.Equal(new[] { "0.00 zł", "365.50 zł", "811.75 zł", "1 339.60 zł" }, result.DebentureData.DebentureColumns[5].Rows);

			testModel.TOSPercentage = 5.0;
			testModel.Amount = 85;
			testModel.BelkaTax = true;
			result = service.GetDebentureAsync(testModel).Result;
			Assert.Equal(2, result.DebentureInfo.Count);
			Assert.Equal(new Tuple<string, string>("Całkowity zysk", "1 084.60 zł"), result.DebentureInfo[0]);
			Assert.Equal(new Tuple<string, string>("Wysokość rocznego procentu składanego (długosc: 3 lata, kapitalizacja: roczna)", "5.25 %"), result.DebentureInfo[1]);
			Assert.Equal(new[] { "Rok", "Całkowita wartość", "Oprocentowanie", "Odsetki", "Suma odsetek", "Zysk netto" }, result.DebentureData.Head.Select(a => a.Head));
			Assert.Equal(new[] { "0", "1", "2", "3" }, result.DebentureData.DebentureColumns[0].Rows);
			Assert.Equal(new[] { "8 500.00 zł", "8 925.00 zł", "9 371.25 zł", "9 839.60 zł" }, result.DebentureData.DebentureColumns[1].Rows);
			Assert.Equal(new[] { "5 %", "5 %", "5 %", "5 %" }, result.DebentureData.DebentureColumns[2].Rows);
			Assert.Equal(new[] { "425.00 zł", "446.25 zł", "468.35 zł", "0.00 zł" }, result.DebentureData.DebentureColumns[3].Rows);
			Assert.Equal(new[] { "0.00 zł", "425.00 zł", "871.25 zł", "1 339.60 zł" }, result.DebentureData.DebentureColumns[4].Rows);
			Assert.Equal(new[] { "0.00 zł", "295.80 zł", "657.05 zł", "1 084.60 zł" }, result.DebentureData.DebentureColumns[5].Rows);

			testModel.TOSPercentage = 6.70;
			testModel.Amount = 33;
			testModel.BelkaTax = false;
			result = service.GetDebentureAsync(testModel).Result;
			Assert.Equal(2, result.DebentureInfo.Count);
			Assert.Equal(new Tuple<string, string>("Całkowity zysk", "708.84 zł"), result.DebentureInfo[0]);
			Assert.Equal(new Tuple<string, string>("Wysokość rocznego procentu składanego (długosc: 3 lata, kapitalizacja: roczna)", "7.16 %"), result.DebentureInfo[1]);
			Assert.Equal(new[] { "Rok", "Całkowita wartość", "Oprocentowanie", "Odsetki", "Suma odsetek", "Zysk netto" }, result.DebentureData.Head.Select(a => a.Head));
			Assert.Equal(new[] { "0", "1", "2", "3" }, result.DebentureData.DebentureColumns[0].Rows);
			Assert.Equal(new[] { "3 300.00 zł", "3 521.10 zł", "3 757.05 zł", "4 008.84 zł" }, result.DebentureData.DebentureColumns[1].Rows);
			Assert.Equal(new[] { "6.7 %", "6.7 %", "6.7 %", "6.7 %" }, result.DebentureData.DebentureColumns[2].Rows);
			Assert.Equal(new[] { "221.10 zł", "235.95 zł", "251.79 zł", "0.00 zł" }, result.DebentureData.DebentureColumns[3].Rows);
			Assert.Equal(new[] { "0.00 zł", "221.10 zł", "457.05 zł", "708.84 zł" }, result.DebentureData.DebentureColumns[4].Rows);
			Assert.Equal(new[] { "0.00 zł", "198.00 zł", "433.95 zł", "708.84 zł" }, result.DebentureData.DebentureColumns[5].Rows);

			testModel.TOSPercentage = 6.70;
			testModel.Amount = 33;
			testModel.BelkaTax = true;
			result = service.GetDebentureAsync(testModel).Result;
			Assert.Equal(2, result.DebentureInfo.Count);
			Assert.Equal(new Tuple<string, string>("Całkowity zysk", "573.87 zł"), result.DebentureInfo[0]);
			Assert.Equal(new Tuple<string, string>("Wysokość rocznego procentu składanego (długosc: 3 lata, kapitalizacja: roczna)", "7.16 %"), result.DebentureInfo[1]);
			Assert.Equal(new[] { "Rok", "Całkowita wartość", "Oprocentowanie", "Odsetki", "Suma odsetek", "Zysk netto" }, result.DebentureData.Head.Select(a => a.Head));
			Assert.Equal(new[] { "0", "1", "2", "3" }, result.DebentureData.DebentureColumns[0].Rows);
			Assert.Equal(new[] { "3 300.00 zł", "3 521.10 zł", "3 757.05 zł", "4 008.84 zł" }, result.DebentureData.DebentureColumns[1].Rows);
			Assert.Equal(new[] { "6.7 %", "6.7 %", "6.7 %", "6.7 %" }, result.DebentureData.DebentureColumns[2].Rows);
			Assert.Equal(new[] { "221.10 zł", "235.95 zł", "251.79 zł", "0.00 zł" }, result.DebentureData.DebentureColumns[3].Rows);
			Assert.Equal(new[] { "0.00 zł", "221.10 zł", "457.05 zł", "708.84 zł" }, result.DebentureData.DebentureColumns[4].Rows);
			Assert.Equal(new[] { "0.00 zł", "160.38 zł", "351.45 zł", "573.87 zł" }, result.DebentureData.DebentureColumns[5].Rows);
		}

		[Fact]
		public void TOSServiceRenderTest()
		{
			var ctx = new TestContext();
			ctx.Services.AddSingleton(new DebentureService());
			var cut = ctx.RenderComponent<DebenturesCalculator>();

			var debentureTypeInput = cut.Find("select");
			debentureTypeInput.Change(DebentureType.TOS);
			var allButtons = cut.FindAll("button");

			var calculateButton = allButtons.Where(x => x.TextContent.Equals("Oblicz")).FirstOrDefault();
			Assert.NotNull(calculateButton);
			var inputs = cut.FindAll("input");
			Assert.Equal(4, inputs.Count);

			var belkaTaxInput = inputs[1];
			var amountInput = inputs[2];
			var percentageInput = inputs[3];

			belkaTaxInput.Input(true);
			amountInput.Change(100);
			calculateButton.Click();

			Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Całkowity zysk")).Any());
			Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Wysokość rocznego procentu składanego (długosc: 3 lata, kapitalizacja: roczna)")).Any());

			var output = cut.FindAll("th");
			Assert.Equal("Rok", output[0].TextContent);
			Assert.Equal("Całkowita wartość", output[1].TextContent);
			Assert.Equal("Oprocentowanie", output[2].TextContent);
			Assert.Equal("Odsetki", output[3].TextContent);
			Assert.Equal("Suma odsetek", output[4].TextContent);
			Assert.Equal("Zysk netto", output[5].TextContent);
		}
	}
}
