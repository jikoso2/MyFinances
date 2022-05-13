using MyFinances.Helpers;
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

		[Range(0.001, 30, ErrorMessage = "Wysokość oprocentowania nie może być mniejsza od zera lub większa od 30")]
		public double OTSPercentage { get; set; } = DefaultValue.OTSPercentage;

		[Range(0.001, 30, ErrorMessage = "Wysokość oprocentowania nie może być mniejsza od zera lub większa od 30")]
		public double DOSPercentage { get; set; } = DefaultValue.DOSPercentage;

		[Required]
		public bool BelkaTax { get; set; }
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
