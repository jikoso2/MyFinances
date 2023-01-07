using Bunit;
using MyFinances.Pages;
using MyFinances.Data;
using Microsoft.Extensions.DependencyInjection;
using MyFinances.Data.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Components.Authorization;
using MyFinances.Data.Auth;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace MyFinanceTests
{
	public class MailServiceTests
	{

		[Fact]
		public void RenderServiceTest()
		{
			var configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json", optional: false).Build();

			var ctx = new TestContext();

			ctx.Services.AddAuthentication();
			ctx.Services.AddAuthorization();
			ctx.Services.AddTransient<IConfiguration>(config => configuration);

			ctx.Services.AddSingleton(new MailService());
			ctx.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
			ctx.Services.AddScoped<DataBaseConnService>();
			ctx.Services.AddScoped<ProtectedSessionStorage>();
			ctx.Services.AddScoped<AuthenticationStateProvider, StateProvider>();

			var cut = ctx.RenderComponent<ContactPage>();

			var labels = cut.FindAll("label");
			Assert.Equal(2, labels.Count);
			Assert.Equal("Email", labels[0].TextContent);
			Assert.Equal("Treść Wiadomości", labels[1].TextContent);

			var inputs = cut.FindAll("input");
			Assert.Equal(1, inputs.Count);
			Assert.Equal("form-control", inputs[0].ClassName);

			var areas = cut.FindAll("textarea");
			Assert.Equal("form-control", inputs[0].ClassName);
			Assert.Equal("input", inputs[0].LocalName);

			var allButtons = cut.FindAll("button");
			Assert.NotNull(allButtons[0]);
			Assert.Equal("Wyślij wiadomość E-Mail", allButtons[0].TextContent);
		}
	}
}