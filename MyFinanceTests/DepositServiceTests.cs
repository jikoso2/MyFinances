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
			inputs[2].Input(1000);
			inputs = cut.FindAll("input");
			inputs[3].Input(12);
			inputs = cut.FindAll("input");
			inputs[4].Input(5);
			inputs = cut.FindAll("input");
			inputs[5].Input(3);

			var allButtons = cut.FindAll("button");
			Assert.NotNull(allButtons[0]);
			allButtons[0].Click();

			var output = cut.FindAll("th");
			Assert.Equal("Okres", output[0].TextContent);
			Assert.Equal("Wypłata", output[1].TextContent);
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
			inputs[2].Input(1000);
			inputs = cut.FindAll("input");
			inputs[3].Input(12);
			inputs = cut.FindAll("input");
			inputs[4].Input(5);
			inputs = cut.FindAll("input");
			inputs[5].Input(3);

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
		public void ServiceTest()
		{


		}
	}
}