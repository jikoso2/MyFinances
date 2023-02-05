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
					return "Trzymiesięczne obligacje ze stałym oprocentowaniem.<br/> Wypłata odsetek: na koniec okresu rozliczeniowego <br/> Koszt wcześniejszego wykupu: 0 gr.";
				case DebentureType.DOS:
					return "Dwuletnie obligacje ze stałym oprocentowaniem z roczną kapitalizacją odsetek. <br/> Wypłata odsetek: na koniec okresu rozliczeniowego. <br/> Koszt wcześniejszego wykupu: 0.70 zł.";
				case DebentureType.TOZ:
					return "Trzyletnie obligacje ze zmiennym oprocentowaniem.<br/> Wypłata odsetek: co pół roku. <br/> Koszt wcześniejszego wykupu: 0.70 zł.";
				case DebentureType.COI:
					return "Czteroletnie obligacje ze zmiennym oprocentowaniem. <br/> Wypłata odsetek: co miesiąc. <br/> Koszt wcześniejszego wykupu: 0.70 zł.";
				case DebentureType.EDO:
					return "Dziesięcioletnie obligacje ze zmiennym oprocentowaniem z co roczną kapitalizacją odsetek <br/> Wypłata odsetek: na koniec okresu rozliczeniowego <br/> Koszt wcześniejszego wykupu: 2 zł.";
				case DebentureType.ROR:
					return "Roczne obligacje ze zmiennym oprocentowaniem indeksowanym wskaźnikiem stopy referencyjnej. <br/> Wypłata odsetek: co miesiąc. <br/> Koszt wcześniejszego wykupu: 0.50 zł.";
				case DebentureType.DOR:
					return "Dwuletnie obligacje ze zmiennym oprocentowaniem indeksowanym wskaźnikiem stopy referencyjnej z dodatkiem 0.10 punkta procentowego.<br/> Wypłata odsetek: co miesiąc. <br/> Koszt wcześniejszego wykupu: 0.70 zł.";
				case DebentureType.TOS:
					return "Trzyletnie obligacje ze stałym oprocentowaniem z roczną kapitalizacją odsetek <br/> Wypłata odsetek: na koniec okresu rozliczeniowego <br/> Koszt wcześniejszego wykupu: 0.70 zł.";
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

		public static string GetPPKPayoutInformation()
        {
			return $"Obliczenia odbywają się przy założeniu wpłat 2% pracownik, 1,5% pracodawca. <br/><br/>" +
				$"Kwota zgromadzona na PPK - suma zgromadzonych pieniędzy (ich aktualna wartość na koncie). <br/>" +
				$"Kwota wpłacona przez państwo - Dokładna kwota wpłacona jako bonus za prowadzenie PPK - niezależnie od wartości funszów</br>" +
				$"Procent zwrotu - procent wyliczony przez obsługującego PPK - wartość ta może być ujemna</br>" +
				$"</br>" +
				$"Kwota wpłacona przez pracodawce - dokładna kwota wpłacona przez pracodawce bez uwzględnienia oprocentowania zwrotu</br>" +
				$"Kwota wpłacona przez pracownika - dokładna kwota wpłacona przez pracownika bez uwzględnienia oprocentowania zwrotu</br>";
		}
	}
}
