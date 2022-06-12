using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinances.Models
{
	public class PPKModel
	{
		[Required]
		[Range(0.001, 30, ErrorMessage = "Wysokość oprocentowania nie może być mniejsza od zera lub większa od 30")]
		public double EmployeePercentage { get; set; } = 2;

		[Required]
		[Range(0.001, 30, ErrorMessage = "Wysokość oprocentowania nie może być mniejsza od zera lub większa od 30")]
		public double EmployerPercentage { get; set; } = 1.5;

		[Required]
		[Range(1000, 10000000, ErrorMessage = "Wysokość wynagrodzenia brutto musi być większa od 1000 zł")]
		public long Amount { get; set; } = 5000;
	}
}
