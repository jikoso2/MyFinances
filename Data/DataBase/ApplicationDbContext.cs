using Microsoft.EntityFrameworkCore;

namespace MyFinances.Data.DataBase
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{

		}

		public DbSet<UserAccount> user { get; set; }

		public DbSet<LoanCalculation> loan_calculation { get; set; }
	}
}
