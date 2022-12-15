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
		public void OTSServiceTest()
		{
			var service = new DebentureService();
			var testModel = new DebentureModel();

			var result = service.GetDebentureAsync(testModel).Result;

			Assert.NotNull(result);
			Assert.Equal(6, result.DebentureData.DebentureColumns.Length);
			Assert.Equal(6, result.DebentureData.Head.Length);
			Assert.Single(result.DebentureInfo);

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

			if (inputs.Any() && calculateButton != null)
			{
				var amountInput = inputs[0];
				var percentageInput = inputs[1];

				amountInput.Change(100);
				percentageInput.Change(3.25);
				calculateButton.Click();

				var output = cut.FindAll("th");
				Assert.Equal("Miesiąc", output[0].TextContent);
				Assert.Equal("Całkowita wartość", output[1].TextContent);
				Assert.Equal("Oprocentowanie", output[2].TextContent);
				Assert.Equal("Odsetki", output[3].TextContent);
				Assert.Equal("Podatek", output[4].TextContent);
				Assert.Equal("Zysk netto", output[5].TextContent);

				Assert.NotNull(cut.FindAll("div").Select(a => a.TextContent == "Całkowity zysk"));
				Assert.NotNull(cut.FindAll("div").Select(a => a.TextContent == "<div class=\"col - 5\">Całkowity zysk</div>"));

			}
		}

		[Fact]
		public void CorrectRenderViews()
		{
			var ctx = new TestContext();
			ctx.Services.AddSingleton(new DebentureService());
			var cut = ctx.RenderComponent<DebenturesCalculator>();

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
	}
}