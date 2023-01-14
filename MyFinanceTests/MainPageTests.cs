using Bunit;
using MyFinances.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MyFinances.Data.DataBase;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Authorization;
using MyFinances.Data.Auth;
using Microsoft.EntityFrameworkCore;

namespace MyFinanceTests
{
	public class MainPageTests
	{
		[Fact]
		public void RenderServiceTest()
		{
			var configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json", optional: false).Build();

			var ctx = new TestContext();

			ctx.Services.AddAuthentication();
			ctx.Services.AddAuthorization();
			ctx.Services.AddTransient<IConfiguration>(config => configuration);

			ctx.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
			ctx.Services.AddScoped<DataBaseConnService>();
			ctx.Services.AddScoped<ProtectedSessionStorage>();
			ctx.Services.AddScoped<AuthenticationStateProvider, StateProvider>();

			var cut = ctx.RenderComponent<MyFinances.Pages.Index>();

			var divs = cut.FindAll("div");
			Assert.Equal("Cześć!", cut.FindAll("h1").First().TextContent);
			Assert.Equal("\n    Chcesz się ze mną skontaktować ?\n\n    \n        Użyj\n         formularza kontaktowego \n        na mojej stronie\n    \n    i napisz do mnie wiadomość.\n\n", divs.FirstOrDefault(a => a.ClassName.Equals("alert alert-secondary mt-4")).TextContent);
		}
	}
}