using MyFinances.Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyFinances.Helpers
{
	public class TogglePayoutType
	{
		public PayoutType[] Options { get; set; }
		public PayoutType SelectedOption { get; set; }

		public string GetActive(PayoutType option)
		{
			return option == SelectedOption ? "active" : "";
		}
	}

}
