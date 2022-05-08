using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFinances.Models;

namespace MyFinances.Helpers
{
	public static class HelperInformations
	{
		public static string GetInformation(DebentureType type)
		{
			switch (type)
			{
				case DebentureType.OTS:
					return "Trzymiesięczne obligacje ze stałym oprocentowaniem z kapitalizacją na koniec okresu rozliczeniowego";
				case DebentureType.DOS:
					return "Dwuletnie obligacje ze stałym oprocentowaniem z roczną kapitalizacją odsetek";
				case DebentureType.TOZ:
					return "Trzyletnie obligacje ze zmiennym oprocentowaniem z pół roczną kapitalizacją odsetek";
				case DebentureType.COI:
					return "Czteroletnie obligacje ze zmiennym oprocentowaniem z co roczną wypłatą odsetek";
				case DebentureType.EDO:
					return "Dziesięcioletnie obligacje ze zmiennym oprocentowaniem z co roczną kapitalizacją odsetek";
				default:
					return "Podstawowe Informację dotyczące typu obligacji";
			}
		}
	}
}
