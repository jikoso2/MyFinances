using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MyFinances.Models
{
	public class ContactMessageModel
	{
		[Required(ErrorMessage = "Wprowadź treść wiadomości")]
		public string EmailMessage { get; set; }

		[Required(ErrorMessage = "Wprowadź adres e-mail")]
		[EmailAddress(ErrorMessage = "Wprowadź poprawny adres e-mail")]
		public string Email { get; set; }
	}
}

