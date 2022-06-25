using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MyFinances.Models
{
	public class ContactMessageModel
	{
		public string EmailMessage { get; set; }

		[Required]
		[EmailAddress]
		public string Email { get; set; }
	}
}

