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
		[Range(1,10000,ErrorMessage = "Liczba zakupionych obligacji musi być całkowita i dodatnia")]
		public int Amount { get; set; }

		[Required]
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
