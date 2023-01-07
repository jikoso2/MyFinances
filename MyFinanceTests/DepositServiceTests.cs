using Bunit;
using MyFinances.Pages;
using MyFinances.Data;
using MyFinances.Models;
using Microsoft.Extensions.DependencyInjection;

namespace MyFinanceTests
{
	public class DepositServiceTests
	{
		[Fact]
		public void RenderServiceTest()
		{
			var ctx = new TestContext();
			ctx.Services.AddSingleton(new DepositService());
			var cut = ctx.RenderComponent<DepositCalculator>();

			var select = cut.FindAll("select");
			Assert.Equal(1, select.Count);
			Assert.Equal("DzieńMiesiącRok", select[0].TextContent);

			var inputs = cut.FindAll("input");
			Assert.Equal(6, inputs.Count);
			Assert.Equal("form-check-label valid", inputs[0].ClassName);
			Assert.Equal("form-check-label valid", inputs[1].ClassName);
			Assert.Equal("form-control", inputs[2].ClassName);
			Assert.Equal("form-control", inputs[3].ClassName);
			Assert.Equal("form-control", inputs[4].ClassName);
			Assert.Equal("form-control", inputs[5].ClassName);

			var labels = cut.FindAll("label");
			Assert.Equal(6, labels.Count);
			Assert.Equal("Kapitalizacja", labels[0].TextContent);
			Assert.Equal("Podatek 19%", labels[1].TextContent);
			Assert.Equal("Kwota ulokowana na lokacie", labels[2].TextContent);
			Assert.Equal("Czas trwania lokaty", labels[3].TextContent);
			Assert.Equal("Roczne oprocentowanie", labels[4].TextContent);
			Assert.Equal("Okres kapitalizacji", labels[5].TextContent);

			var allButtons = cut.FindAll("button");
			Assert.NotNull(allButtons[0]);
		}

		[Fact]
		public void RenderServiceWithoutCapitalizationTest()
		{
			var ctx = new TestContext();
			ctx.Services.AddSingleton(new DepositService());
			var cut = ctx.RenderComponent<DepositCalculator>();

			var inputs = cut.FindAll("input");
			inputs = cut.FindAll("input");
			inputs[0].Change(false);
			inputs[2].Change(1000);
			inputs = cut.FindAll("input");
			inputs[3].Change(12);
			inputs = cut.FindAll("input");
			inputs[4].Change(5);
			inputs = cut.FindAll("input");
			inputs[5].Change(3);

			var allButtons = cut.FindAll("button");
			Assert.NotNull(allButtons[0]);
			allButtons[0].Click();

			var output = cut.FindAll("th");
			Assert.Equal("Okres", output[0].TextContent);
			Assert.Equal("Wypłacone odsetki", output[1].TextContent);
			Assert.Equal("Zysk przy wypłacie", output[2].TextContent);

			Assert.NotNull(cut.FindAll("div").Select(a => a.TextContent == "Kwota na lokacie"));
			Assert.NotNull(cut.FindAll("div").Select(a => a.TextContent == "<div class=\"col - 5\">Kwota na lokacie</div>"));
			Assert.NotNull(cut.FindAll("div").Select(a => a.TextContent == "Ilość okresów rozliczeniowych"));
			Assert.NotNull(cut.FindAll("div").Select(a => a.TextContent == "<div class=\"col - 5\">Ilość okresów rozliczeniowych</div>"));
		}

		[Fact]
		public void RenderServiceWithCapitalizationTest()
		{
			var ctx = new TestContext();
			ctx.Services.AddSingleton(new DepositService());
			var cut = ctx.RenderComponent<DepositCalculator>();

			var inputs = cut.FindAll("input");
			inputs[0].Change(true);
			inputs = cut.FindAll("input");
			inputs[2].Change(1000);
			inputs = cut.FindAll("input");
			inputs[3].Change(12);
			inputs = cut.FindAll("input");
			inputs[4].Change(5);
			inputs = cut.FindAll("input");
			inputs[5].Change(3);

			var allButtons = cut.FindAll("button");
			Assert.NotNull(allButtons[0]);
			allButtons[0].Click();

			var output = cut.FindAll("th");
			Assert.Equal("Okres", output[0].TextContent);
			Assert.Equal("Kapitalizowane odsetki", output[1].TextContent);
			Assert.Equal("Zysk przy wypłacie", output[2].TextContent);

			Assert.NotNull(cut.FindAll("div").Select(a => a.TextContent == "Kwota na lokacie"));
			Assert.NotNull(cut.FindAll("div").Select(a => a.TextContent == "<div class=\"col - 5\">Kwota na lokacie</div>"));
			Assert.NotNull(cut.FindAll("div").Select(a => a.TextContent == "Ilość okresów rozliczeniowych"));
			Assert.NotNull(cut.FindAll("div").Select(a => a.TextContent == "<div class=\"col - 5\">Ilość okresów rozliczeniowych</div>"));
		}

		[Fact]
		public void ServiceTestScenario1()
		{
			var service = new DepositService();
			var testModel = new DepositModel()
			{
				Amount = 30000,
				BelkaTax = false,
				Capitalization = true,
				Duration = 10,
				Period = 2,
				DurationType = TimeType.Miesiąc,
				Percentage = 3.54
			};

			var result = service.GetDepositAsync(testModel).Result;

			Assert.Equal("Kwota na lokacie", result.DepositInfo[0].Item1);
			Assert.Equal("30 000.00 zł", result.DepositInfo[0].Item2);
			Assert.Equal("Ilość okresów rozliczeniowych", result.DepositInfo[1].Item1);
			Assert.Equal("5", result.DepositInfo[1].Item2);

			Assert.Equal(new string[] { "Okres", "Kapitalizowane odsetki", "Zysk przy wypłacie" }, result.DepositData.Head);
			Assert.Equal(new string[] { "1", "2", "3", "4", "5" }, result.DepositData.DepositColumn[0].Rows);
			Assert.Equal(new string[] { "177.00 zł", "178.04 zł", "179.09 zł", "180.15 zł", "181.21 zł" }, result.DepositData.DepositColumn[1].Rows);
			Assert.Equal(new string[] { "177.00 zł", "355.04 zł", "534.13 zł", "714.28 zł", "895.49 zł" }, result.DepositData.DepositColumn[2].Rows);
		}

		[Fact]
		public void ServiceTestScenario2()
		{
			var service = new DepositService();
			var testModel = new DepositModel()
			{
				Amount = 30000,
				BelkaTax = true,
				Capitalization = true,
				Duration = 10,
				Period = 2,
				DurationType = TimeType.Miesiąc,
				Percentage = 3.54
			};

			var result = service.GetDepositAsync(testModel).Result;

			Assert.Equal("Kwota na lokacie", result.DepositInfo[0].Item1);
			Assert.Equal("30 000.00 zł", result.DepositInfo[0].Item2);
			Assert.Equal("Ilość okresów rozliczeniowych", result.DepositInfo[1].Item1);
			Assert.Equal("5", result.DepositInfo[1].Item2);

			Assert.Equal(new string[] { "Okres", "Kapitalizowane odsetki", "Zysk przy wypłacie" }, result.DepositData.Head);
			Assert.Equal(new string[] { "1", "2", "3", "4", "5" }, result.DepositData.DepositColumn[0].Rows);
			Assert.Equal(new string[] { "143.37 zł", "144.05 zł", "144.74 zł", "145.43 zł", "146.13 zł" }, result.DepositData.DepositColumn[1].Rows);
			Assert.Equal(new string[] { "143.37 zł", "287.42 zł", "432.16 zł", "577.59 zł", "723.72 zł" }, result.DepositData.DepositColumn[2].Rows);
		}

		[Fact]
		public void ServiceTestScenario3()
		{
			var service = new DepositService();
			var testModel = new DepositModel()
			{
				Amount = 30000,
				BelkaTax = false,
				Capitalization = false,
				Duration = 30,
				Period = 15,
				DurationType = TimeType.Dzień,
				Percentage = 6.45
			};

			var result = service.GetDepositAsync(testModel).Result;

			Assert.Equal("Kwota na lokacie", result.DepositInfo[0].Item1);
			Assert.Equal("30 000.00 zł", result.DepositInfo[0].Item2);
			Assert.Equal("Ilość okresów rozliczeniowych", result.DepositInfo[1].Item1);
			Assert.Equal("2", result.DepositInfo[1].Item2);

			Assert.Equal(new string[] { "Okres", "Wypłacone odsetki", "Zysk przy wypłacie" }, result.DepositData.Head);
			Assert.Equal(new string[] { "1", "2" }, result.DepositData.DepositColumn[0].Rows);
			Assert.Equal(new string[] { "79.52 zł", "79.52 zł" }, result.DepositData.DepositColumn[1].Rows);
			Assert.Equal(new string[] { "79.52 zł", "159.04 zł" }, result.DepositData.DepositColumn[2].Rows);
		}

		[Fact]
		public void ServiceTestScenario4()
		{
			var service = new DepositService();
			var testModel = new DepositModel()
			{
				Amount = 30000,
				BelkaTax = true,
				Capitalization = false,
				Duration = 30,
				Period = 15,
				DurationType = TimeType.Dzień,
				Percentage = 6.45
			};

			var result = service.GetDepositAsync(testModel).Result;

			Assert.Equal("Kwota na lokacie", result.DepositInfo[0].Item1);
			Assert.Equal("30 000.00 zł", result.DepositInfo[0].Item2);
			Assert.Equal("Ilość okresów rozliczeniowych", result.DepositInfo[1].Item1);
			Assert.Equal("2", result.DepositInfo[1].Item2);

			Assert.Equal(new string[] { "Okres", "Wypłacone odsetki", "Zysk przy wypłacie" }, result.DepositData.Head);
			Assert.Equal(new string[] { "1", "2" }, result.DepositData.DepositColumn[0].Rows);
			Assert.Equal(new string[] { "64.41 zł", "64.41 zł" }, result.DepositData.DepositColumn[1].Rows);
			Assert.Equal(new string[] { "64.41 zł", "128.82 zł" }, result.DepositData.DepositColumn[2].Rows);
		}
	}
}