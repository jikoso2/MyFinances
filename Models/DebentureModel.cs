using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinances.Models
{
	public class DebentureModel
	{
		[Required]
		[Range(1,10000,ErrorMessage = "Liczba zakupionych obligacji musi być całkowita i dodatnia")]
		public int Amount { get; set; }

		[Required]
		public DebentureType Type { get; set; }
		public double OTSPercentage { get; set; } = 1.5;
	}

	public enum DebentureType 
	{ 
		OTS,
		DOS,
		TOZ,
		COI,
		EDO
	}
}
