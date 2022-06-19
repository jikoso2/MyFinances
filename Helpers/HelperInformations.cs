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
					return "Trzymiesięczne obligacje ze stałym oprocentowaniem z kapitalizacją na koniec okresu rozliczeniowego. <br/> Koszt wcześniejszego wykupu: 0 gr, został uwzględniony w obliczeniach.";
				case DebentureType.DOS:
					return "Dwuletnie obligacje ze stałym oprocentowaniem z roczną kapitalizacją odsetek.";
				case DebentureType.TOZ:
					return "Trzyletnie obligacje ze zmiennym oprocentowaniem z pół roczną kapitalizacją odsetek. <br/> Koszt wcześniejszego wykupu: 70 gr, został uwzględniony w obliczeniach.";
				case DebentureType.COI:
					return "Czteroletnie obligacje ze zmiennym oprocentowaniem z co roczną wypłatą odsetek. <br/> Koszt wcześniejszego wykupu: 70 gr.";
				case DebentureType.EDO:
					return "Dziesięcioletnie obligacje ze zmiennym oprocentowaniem z co roczną kapitalizacją odsetek <br/> Koszt wcześniejszego wykupu: 2 zł, został uwzględniony w obliczeniach.";
				case DebentureType.ROR:
					return "Roczne obligacje ze zmiennym oprocentowaniem indeksowanym wskaźnikiem stopy referencyjnej. Odsetki wypłacane są co miesiąc. <br/> Koszt wcześniejszego wykupu: 50 gr.";
				case DebentureType.DOR:
					return "Dwuletnie obligacje ze zmiennym oprocentowaniem indeksowanym wskaźnikiem stopy referencyjnej z dodatkiem 0.25 punkta procentowego. <br/> Koszt wcześniejszego wykupu: 70 gr.";
				default:
					return "Podstawowe Informację dotyczące typu obligacji.";
			}
		}

		public static string GetPPKInformation()
		{
			return $"Pracowicze Plany Kapitałowe umożliwiają odkładanie na emeryture a także nieznaczne zwiększenie zysków z pracy.<br/>" +
				$" Pracodawca jest zobowiązany do ﬁnansowania wpłat podstawowych do PPK w wysokości 1,5% wynagrodzenia pracownika. Może ﬁnansować wpłaty dodatkowe w wysokości do 2,5% wynagrodzenia (łącznie maksymalnie 4%).<br/>" +
				$"Pracownik co miesiąc przeznacza na PPK 2 % swojego wynagrodzenia. Może także zadeklarować finansowanie wpłaty dodatkowej – w wysokości do 2 % wynagrodzenia(łącznie maksymalnie 4 %).<br/>" +
				$"Pracownik może zdecydować o wypłacie środków przed 60 rokiem życia, wtedy musi opłacić należny podatek od wypracowanego zysku, a także 30% wpłat pracodawcy przekazać do ZUSu";
		}
	}
}
