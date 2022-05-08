﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinances.Models
{
	public class LoanModel
	{
		[Required]
		[Range(1,100000000,ErrorMessage = "Wysokość kredytu nie może być mniejsza od zera lub większa od 10 milionów")]
		public long Amount { get; set; }

		[Required]
		[Range(0.001,30,ErrorMessage ="Wysokość oprocentowania nie może być mniejsza od zera lub większa od 30")]
		public double Percentage { get; set; }

		[Required]
		[Range(12,360,ErrorMessage ="Długość kredytu musi zawierać się w przedziale od 12 do 360 miesięcy")]
		public int Duration { get; set; }

		public double PercentageNumber { get { return Percentage / 100; } }
	}
}