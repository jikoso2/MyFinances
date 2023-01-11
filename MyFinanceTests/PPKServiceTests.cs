using Bunit;
using MyFinances.Pages;
using MyFinances.Data;
using MyFinances.Models;
using Microsoft.Extensions.DependencyInjection;

namespace MyFinanceTests
{
	public class PPKServiceTests
	{
		[Fact]
		public void RenderServiceTest()
		{
			var ctx = new TestContext();
			ctx.Services.AddSingleton(new PPKService());
			var cut = ctx.RenderComponent<PPKCalculator>();

			var inputs = cut.FindAll("input");
			Assert.Equal(9, inputs.Count);
			Assert.Equal("form-control", inputs[0].ClassName);
			Assert.Equal("form-check-label valid", inputs[1].ClassName);
			Assert.Equal("form-control", inputs[2].ClassName);
			Assert.Equal("form-range", inputs[3].ClassName);
			Assert.Equal("form-control", inputs[4].ClassName);
			Assert.Equal("form-range", inputs[5].ClassName);
			Assert.Equal("form-control", inputs[6].ClassName);
			Assert.Equal("form-range", inputs[7].ClassName);
			Assert.Equal("form-control", inputs[8].ClassName);

			var labels = cut.FindAll("label");
			Assert.Equal(6, labels.Count);
			Assert.Equal("Wysokość wypłaty brutto", labels[0].TextContent);
			Assert.Equal("Wypłata przed 60 rokiem życia", labels[1].TextContent);
			Assert.Equal("Długość oszczędzania", labels[2].TextContent);
			Assert.Equal("Procent wypłaty wpłacany przez pracownika", labels[3].TextContent);
			Assert.Equal("Procent wypłaty wpłacany przez pracodawce", labels[4].TextContent);
			Assert.Equal("Szacowane roczne oprocentowanie funduszu", labels[5].TextContent);

			var allButtons = cut.FindAll("button");
			Assert.NotNull(allButtons[0]);
			Assert.Equal("Informacje dotyczące obliczania PPK", allButtons[0].TextContent);
			Assert.NotNull(allButtons[1]);
			Assert.Equal("Oblicz", allButtons[1].TextContent);

			inputs = cut.FindAll("input");
			inputs[1].Change(false);
			allButtons = cut.FindAll("button");
			allButtons[1].Click();

			Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Miesięczna wysokość wpłaty pracownika")).Any());
			Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Miesięczna wysokość wpłaty pracodawcy")).Any());
			Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Ilość okresów")).Any());
			Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Zgromadzony kapitał")).Any());
			Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Wielkość odsetek w kapitale")).Any());

			inputs = cut.FindAll("input");
			inputs[1].Change(true);
			allButtons[1].Click();

			Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Podatek od zgromadzonych odsetek")).Any());
			Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Część odprowadzona do ZUSu")).Any());
			Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Kwota wypłaty")).Any());
			Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Zysk netto przy wcześniejszej wypłacie")).Any());
		}


		[Fact]
		public void ServiceTestScenario1()
		{
			var service = new PPKService();
			var testModel = new PPKModel()
			{
				Amount = 30000,
				EarlyPayment = true,
				Duration = 15,
				DepositPercentage = 2,
				EmployeePercentage = 2,
				EmployerPercentage = 1.5
			};

			var result = service.GetPPKCalculatedAsync(testModel).Result;

			Assert.Equal("Miesięczny koszt pracownika (+12% podatek dochodowy)", result.PPKInfo[0].Item1);
			Assert.Equal("654.00 zł", result.PPKInfo[0].Item2);
			Assert.Equal("Miesięczna wysokość wpłaty pracownika", result.PPKInfo[1].Item1);
			Assert.Equal("600.00 zł", result.PPKInfo[1].Item2);

		}
	}
}