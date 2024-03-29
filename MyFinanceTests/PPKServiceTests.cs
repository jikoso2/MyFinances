﻿using Bunit;
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

			Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Zgromadzone odsetki z wpłat pracownika minus podatek")).Any());
			Assert.True(cut.FindAll("div").Where(a => a.TextContent.Equals("Zgromadzone odsetki z wpłat pracodawcy minus podatek")).Any());
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
			Assert.Equal("Miesięczna wysokość wpłaty pracodawcy", result.PPKInfo[2].Item1);
			Assert.Equal("450.00 zł", result.PPKInfo[2].Item2);
			Assert.Equal("Ilość okresów", result.PPKInfo[3].Item1);
			Assert.Equal("15", result.PPKInfo[3].Item2);
			Assert.Equal("Zgromadzony kapitał", result.PPKInfo[4].Item1);
			Assert.Equal("15 935.08 zł", result.PPKInfo[4].Item2);
			Assert.Equal("Wielkość odsetek w kapitale", result.PPKInfo[5].Item1);
			Assert.Equal("185.08 zł", result.PPKInfo[5].Item2);
			Assert.Equal("Zgromadzone odsetki z wpłat pracownika minus podatek", result.PPKInfo[6].Item1);
			Assert.Equal("45.12 zł", result.PPKInfo[6].Item2);
			Assert.Equal("Zgromadzone odsetki z wpłat pracodawcy minus podatek", result.PPKInfo[7].Item1);
			Assert.Equal("85.45 zł", result.PPKInfo[7].Item2);
			Assert.Equal("Część odprowadzona do ZUSu", result.PPKInfo[8].Item1);
			Assert.Equal("2 025.00 zł", result.PPKInfo[8].Item2);
			Assert.Equal("Kwota wypłaty", result.PPKInfo[9].Item1);
			Assert.Equal("13 855.57 zł", result.PPKInfo[9].Item2);
			Assert.Equal("Zysk netto przy wcześniejszej wypłacie", result.PPKInfo[10].Item1);
			Assert.Equal("4 045.57 zł", result.PPKInfo[10].Item2);

		}

		[Fact]
		public void ServiceTestScenario2()
		{
			var service = new PPKService();
			var testModel = new PPKModel()
			{
				Amount = 8000,
				EarlyPayment = true,
				Duration = 2,
				DepositPercentage = 2,
				EmployeePercentage = 2,
				EmployerPercentage = 1.5
			};

			var result = service.GetPPKCalculatedAsync(testModel).Result;

			Assert.Equal("Miesięczny koszt pracownika (+12% podatek dochodowy)", result.PPKInfo[0].Item1);
			Assert.Equal("174.40 zł", result.PPKInfo[0].Item2);
			Assert.Equal("Miesięczna wysokość wpłaty pracownika", result.PPKInfo[1].Item1);
			Assert.Equal("160.00 zł", result.PPKInfo[1].Item2);
			Assert.Equal("Miesięczna wysokość wpłaty pracodawcy", result.PPKInfo[2].Item1);
			Assert.Equal("120.00 zł", result.PPKInfo[2].Item2);
			Assert.Equal("Ilość okresów", result.PPKInfo[3].Item1);
			Assert.Equal("2", result.PPKInfo[3].Item2);
			Assert.Equal("Zgromadzony kapitał", result.PPKInfo[4].Item1);
			Assert.Equal("560.47 zł", result.PPKInfo[4].Item2);
			Assert.Equal("Wielkość odsetek w kapitale", result.PPKInfo[5].Item1);
			Assert.Equal("0.47 zł", result.PPKInfo[5].Item2);
			Assert.Equal("Zgromadzone odsetki z wpłat pracownika minus podatek", result.PPKInfo[6].Item1);
			Assert.Equal("0.11 zł", result.PPKInfo[6].Item2);
			Assert.Equal("Zgromadzone odsetki z wpłat pracodawcy minus podatek", result.PPKInfo[7].Item1);
			Assert.Equal("0.21 zł", result.PPKInfo[7].Item2);
			Assert.Equal("Część odprowadzona do ZUSu", result.PPKInfo[8].Item1);
			Assert.Equal("72.00 zł", result.PPKInfo[8].Item2);
			Assert.Equal("Kwota wypłaty", result.PPKInfo[9].Item1);
			Assert.Equal("488.32 zł", result.PPKInfo[9].Item2);
			Assert.Equal("Zysk netto przy wcześniejszej wypłacie", result.PPKInfo[10].Item1);
			Assert.Equal("139.52 zł", result.PPKInfo[10].Item2);

		}
	}
}