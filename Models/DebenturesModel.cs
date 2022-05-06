using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinances.Models
{
	public class DebenturesModel
	{
		[Required]
		[Range(1,100000000,ErrorMessage = "Wysokość kredytu nie może być mniejsza od zera lub większa od 10 milionów")]
		public long Amount { get; set; }

		[Required]
		[Range(0.001,30,ErrorMessage ="Wysokość oprocentowania nie może być mniejsza od zera lub większa od 30")]
		public DebentureType Type { get; set; }

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
