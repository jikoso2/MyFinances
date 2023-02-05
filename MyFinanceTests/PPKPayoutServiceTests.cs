using Bunit;
using MyFinances.Pages;
using MyFinances.Data;
using MyFinances.Models;
using Microsoft.Extensions.DependencyInjection;

namespace MyFinanceTests
{
	public class PPKPayoutServiceTests
	{
		[Fact]
		public void RenderServiceTest()
		{
			var ctx = new TestContext();
			ctx.Services.AddSingleton(new PPKPayoutService());
			var cut = ctx.RenderComponent<PPKPayoutCalculator>();


			var inputs = cut.FindAll("input");
			inputs[0].Change(PayoutType.Całość);

			Assert.Equal(6, inputs.Count);
			Assert.Equal("form-control", inputs[2].ClassName);
			Assert.Equal("form-control", inputs[3].ClassName);
			Assert.Equal("form-control", inputs[4].ClassName);
			Assert.Equal("form-check-label valid", inputs[5].ClassName);

			var allButtons = cut.FindAll("button");
			Assert.Equal("Informacje dotyczące obliczania wypłat", allButtons[0].TextContent);
			Assert.Equal("Oblicz", allButtons[1].TextContent);

			allButtons[1].Click();
			Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Wypłata części pracownika")).Any());
			Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Wypłata części pracodawcy")).Any());
			Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Wypłata")).Any());

			inputs = cut.FindAll("input");
			inputs[1].Change(PayoutType.Cześciami);
			inputs = cut.FindAll("input");

			Assert.Equal(7, inputs.Count);
			Assert.Equal("form-control", inputs[2].ClassName);
			Assert.Equal("form-control", inputs[3].ClassName);
			Assert.Equal("form-control", inputs[4].ClassName);
			Assert.Equal("form-control", inputs[5].ClassName);
			Assert.Equal("form-check-label valid", inputs[6].ClassName);

			allButtons = cut.FindAll("button");
			Assert.Equal("Informacje dotyczące obliczania wypłat", allButtons[0].TextContent);
			Assert.Equal("Oblicz", allButtons[1].TextContent);

			allButtons[1].Click();
			Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Wypłata części pracownika")).Any());
			Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Wypłata części pracodawcy")).Any());
			Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Wypłata")).Any());
			Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Koszt netto pracownika (uwzględnienie 12% podatku dochodowego)")).Any());
			Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Zysk netto pracownika")).Any());
		}

		[Fact]
		public void ServiceTestScenario1()
		{
			var service = new PPKPayoutService();
			var testModel = new PPKPayoutModel()
			{
				Amount = 7000,
				CountryAmount = 970,
				PayoutType = PayoutType.Całość,
				Percentage = 0
			};

			var result = service.GetPPKPayoutCalculatedAsync(testModel).Result;

			Assert.Equal("Wypłata części pracownika", result.PPKPayoutInfo[0].Item1);
			Assert.Equal("3 445.71 zł", result.PPKPayoutInfo[0].Item2);
			Assert.Equal("Wypłata części pracodawcy", result.PPKPayoutInfo[1].Item1);
			Assert.Equal("1 809.00 zł", result.PPKPayoutInfo[1].Item2);
			Assert.Equal("Wypłata", result.PPKPayoutInfo[2].Item1);
			Assert.Equal("5 254.71 zł", result.PPKPayoutInfo[2].Item2);
		}

		[Fact]
		public void ServiceTestScenario2()
		{
			var service = new PPKPayoutService();
			var testModel = new PPKPayoutModel()
			{
				Amount = 7000,
				CountryAmount = 970,
				PayoutType = PayoutType.Całość,
				Percentage = -10
			};

			var result = service.GetPPKPayoutCalculatedAsync(testModel).Result;

			Assert.Equal("Wypłata części pracownika", result.PPKPayoutInfo[0].Item1);
			Assert.Equal("3 501.14 zł", result.PPKPayoutInfo[0].Item2);
			Assert.Equal("Wypłata części pracodawcy", result.PPKPayoutInfo[1].Item1);
			Assert.Equal("1 838.10 zł", result.PPKPayoutInfo[1].Item2);
			Assert.Equal("Wypłata", result.PPKPayoutInfo[2].Item1);
			Assert.Equal("5 339.24 zł", result.PPKPayoutInfo[2].Item2);
		}

		[Fact]
		public void ServiceTestScenario3()
		{
			var service = new PPKPayoutService();
			var testModel = new PPKPayoutModel()
			{
				Amount = 7000,
				CountryAmount = 970,
				PayoutType = PayoutType.Całość,
				Percentage = 13
			};

			var result = service.GetPPKPayoutCalculatedAsync(testModel).Result;

			Assert.Equal("Wypłata części pracownika", result.PPKPayoutInfo[0].Item1);
			Assert.Equal("3 299.91 zł", result.PPKPayoutInfo[0].Item2);
			Assert.Equal("Wypłata części pracodawcy", result.PPKPayoutInfo[1].Item1);
			Assert.Equal("1 732.45 zł", result.PPKPayoutInfo[1].Item2);
			Assert.Equal("Wypłata", result.PPKPayoutInfo[2].Item1);
			Assert.Equal("5 032.36 zł", result.PPKPayoutInfo[2].Item2);
		}

		[Fact]
		public void ServiceTestScenario4()
		{
			var service = new PPKPayoutService();
			var testModel = new PPKPayoutModel()
			{
				EmployerAmount = 500,
				EmployeeAmount = 375,
				CountryAmount = 250,
				PayoutType = PayoutType.Cześciami,
				Percentage = 0
			};

			var result = service.GetPPKPayoutCalculatedAsync(testModel).Result;

			Assert.Equal("Wypłata części pracownika", result.PPKPayoutInfo[0].Item1);
			Assert.Equal("500.00 zł", result.PPKPayoutInfo[0].Item2);
			Assert.Equal("Wypłata części pracodawcy", result.PPKPayoutInfo[1].Item1);
			Assert.Equal("262.50 zł", result.PPKPayoutInfo[1].Item2);
			Assert.Equal("Wypłata", result.PPKPayoutInfo[2].Item1);
			Assert.Equal("762.50 zł", result.PPKPayoutInfo[2].Item2);
			Assert.Equal("Koszt netto pracownika (uwzględnienie 12% podatku dochodowego)", result.PPKPayoutInfo[3].Item1);
			Assert.Equal("545.00 zł", result.PPKPayoutInfo[3].Item2);
			Assert.Equal("Zysk netto pracownika", result.PPKPayoutInfo[4].Item1);
			Assert.Equal("217.50 zł", result.PPKPayoutInfo[4].Item2);
		}

		[Fact]
		public void ServiceTestScenario5()
		{
			var service = new PPKPayoutService();
			var testModel = new PPKPayoutModel()
			{
				EmployerAmount = 500,
				EmployeeAmount = 375,
				CountryAmount = 250,
				PayoutType = PayoutType.Cześciami,
				Percentage = -10
			};

			var result = service.GetPPKPayoutCalculatedAsync(testModel).Result;

			Assert.Equal("Wypłata części pracownika", result.PPKPayoutInfo[0].Item1);
			Assert.Equal("450.00 zł", result.PPKPayoutInfo[0].Item2);
			Assert.Equal("Wypłata części pracodawcy", result.PPKPayoutInfo[1].Item1);
			Assert.Equal("236.25 zł", result.PPKPayoutInfo[1].Item2);
			Assert.Equal("Wypłata", result.PPKPayoutInfo[2].Item1);
			Assert.Equal("686.25 zł", result.PPKPayoutInfo[2].Item2);
			Assert.Equal("Koszt netto pracownika (uwzględnienie 12% podatku dochodowego)", result.PPKPayoutInfo[3].Item1);
			Assert.Equal("545.00 zł", result.PPKPayoutInfo[3].Item2);
			Assert.Equal("Zysk netto pracownika", result.PPKPayoutInfo[4].Item1);
			Assert.Equal("141.25 zł", result.PPKPayoutInfo[4].Item2);
		}

		[Fact]
		public void ServiceTestScenario6()
		{
			var service = new PPKPayoutService();
			var testModel = new PPKPayoutModel()
			{
				EmployerAmount = 500,
				EmployeeAmount = 375,
				CountryAmount = 250,
				PayoutType = PayoutType.Cześciami,
				Percentage = 13
			};

			var result = service.GetPPKPayoutCalculatedAsync(testModel).Result;

			Assert.Equal("Wypłata części pracownika", result.PPKPayoutInfo[0].Item1);
			Assert.Equal("552.65 zł", result.PPKPayoutInfo[0].Item2);
			Assert.Equal("Wypłata części pracodawcy", result.PPKPayoutInfo[1].Item1);
			Assert.Equal("290.14 zł", result.PPKPayoutInfo[1].Item2);
			Assert.Equal("Wypłata", result.PPKPayoutInfo[2].Item1);
			Assert.Equal("842.79 zł", result.PPKPayoutInfo[2].Item2);
			Assert.Equal("Koszt netto pracownika (uwzględnienie 12% podatku dochodowego)", result.PPKPayoutInfo[3].Item1);
			Assert.Equal("545.00 zł", result.PPKPayoutInfo[3].Item2);
			Assert.Equal("Zysk netto pracownika", result.PPKPayoutInfo[4].Item1);
			Assert.Equal("297.79 zł", result.PPKPayoutInfo[4].Item2);
		}
	}
}