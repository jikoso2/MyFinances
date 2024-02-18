# [Aplikacja Moje Finanse](https://mojefinanse.azurewebsites.net/)
> 
Aplikacja wspomaga wykonywanie wielu obliczeń związanych z finansami osobistymi. Moje Finanse to doskonałe narzędzie dla osób, które chcą mieć pełną kontrolę nad swoimi finansami i zarządzać nimi w sposób efektywny.
Stworzona głównie na użytek mieszkańców Polski ze względu na dostępność obligacji skarbowych a także specyficzne prawo podatkowe.

## Spis Treści
* [Ogólne Informacje](#ogólne-Informacje)
* [Technologie](#technologie)
* [NuGets](#nugets)
* [Funkcje](#Funkcje)
* [Status](#Status)
* [Przewodnik](#Przewodnik)
* [Testy](#Testy)
* [Przykłady Kodu](#Przykłady-Kodu)
* [Baza Danych](#Baza-Danych)
* [Inspiracja](#Inspiracja)
* [Kontakt](#Kontakt)

## Ogólne Informacje

Aplikacja Moje Finanse udostępnia użytkownikowi zestaw narzędzi do zarządzania finansami osobistymi.
Umożliwia wyliczenie zysków z dostępnych w Polsce obligacji skarbowych.
W kalkulatorze kredytowym użytkownik ma możliwość zobaczyć i zrozumieć mechanizmy działające na kredyty a także sprawdzić wpływ nadpłat na harmonogram spłat.
Kalkulator lokat i kont oszczędnościowych pozwala wyliczać przewidywane zyski z inwestycji w tego typu instrumenty.
Każde z obliczeń można wykonać z uwzględnieniem podatku Belki lub bez niego (produkty jak IKE, IKZE, PPK).
Kalkulator PPK pozwala oszacować wysokość wypłaty z PPK w zależności od wpłaconych środków oraz stan konta po określonym okresie oszczędzania.

Aplikacja hostowana jest na platformie Azure, przyjemny proces releasowania nowych wersji:

![image](https://github.com/jikoso2/MyFinances/assets/69644118/9039a3d9-fb17-4933-bfca-3f26fdea4e49)

## Technologie
* .NET Core - version 6.0
* Npgsql 6.0.7
* Microsoft.EntityFrameworkCore 6.0.11

## NuGets
![Nugets](./img_README/nugets.png)

## Funkcje
Lista dostępnych funkcji dla użytkownika
* [Kalkulator Obligacji](https://mojefinanse.azurewebsites.net/debenturescalculator)
* [Kalkulator Kredytowy](https://mojefinanse.azurewebsites.net/loancalculator)
* [Kalkulator Lokat](https://mojefinanse.azurewebsites.net/depositcalculator)
* [Kalkulator Konto Oszczędnościowe (w trakcie rozbudowy)](https://mojefinanse.azurewebsites.net/depositaccountcalculator)
* Kalkulator Procentu Składanego (w trakcie realizacji)
* [Kalkulator PPK](https://mojefinanse.azurewebsites.net/ppkcalculator)
* [Kalkulator Wypłat PPK](https://mojefinanse.azurewebsites.net/ppkpayoutcalculator)
* [Formularz Kontaktowy](https://mojefinanse.azurewebsites.net/contact)
* [Konta użytkowników (Panel Administratora)](https://mojefinanse.azurewebsites.net/users)
* [Konfiguracja aplikacji (Panel Administratora)](https://mojefinanse.azurewebsites.net/configuration)

To-do lista:
* Automatyczna synchronizacja informacji dotyczących obligacji ze stron rządowych (web-scrapping)
* Kalkulator zmian giełdowych
* Export wyników do PDF

## Status

Projekt jest gotowy do użycia.
[MojeFinanse](https://mojefinanse.azurewebsites.net/)
Stale jest rozwijany.

## Przewodnik

![Example Menu](./img_README/menu.png)

### Logowanie
Istnieją dwa typy użytkowników.
* podstawowy - pozwala na korzystanie z zapisanych kalkulacji kredytu
* administrator - umożliwia konfigurowanie domyślnych wartości w aplikacji oraz na zarządzanie użytkownikami
![image](https://github.com/jikoso2/MyFinances/assets/69644118/104c8c1f-ba79-411a-8056-b0a569c0daeb)

### Funkcje Administratorskie
#### Zarządznie kontami użytkowników
![image](https://github.com/jikoso2/MyFinances/assets/69644118/8beb0843-d857-49f6-a51e-a1061b0fd6b7)

Osobny panel do zarządzania użytkownikami, dodatkowo hasła są hashowane tak aby zmienione hasła od użytkowników nie były widoczne dla administratorów.

![image](https://github.com/jikoso2/MyFinances/assets/69644118/4a364c76-742f-4225-9424-a4ac08b0f854)
![image](https://github.com/jikoso2/MyFinances/assets/69644118/781acac1-bee6-4313-a1e5-c28adf77e779)
![image](https://github.com/jikoso2/MyFinances/assets/69644118/779f1223-418b-489b-919b-24cad1992819)
#### Konfigurowanie domyślnych wartości
![image](https://github.com/jikoso2/MyFinances/assets/69644118/4f020460-12e5-45bf-a8b8-e84cd076e92c)

Dodatkowy panel pozwalający na sterowanie domyślnymi wartościami na starcie aplikacji.
Pozwala on na aktualizowanie aplikacji bez konieczności publikowania kolejnych wersji aplikacji. 
np. dane znajdujące się w zakładce kredyt:

![image](https://github.com/jikoso2/MyFinances/assets/69644118/d2b1a394-a6f4-43a6-a4bd-cf02f84a5b94)
![image](https://github.com/jikoso2/MyFinances/assets/69644118/2cf2a322-7b18-4802-a47a-d29a8a4328d4)

### Kalkulator Obligacji
![image](https://github.com/jikoso2/MyFinances/assets/69644118/19cbdbb7-3c23-425a-b1d5-82072a1e07a0)

Kalkulator ten umożliwia kalkulacje zysków ze wszystkich aktualnie dostępnych obligacji oraz tych które zostały wycofane z obiegu.

![image](https://github.com/jikoso2/MyFinances/assets/69644118/d94c29c6-b1a4-4606-bf2a-2503de1be629)

Przykładowa kalkulacja dla czteroletnich obligacji indeksowanych inflacją:

![image](https://github.com/jikoso2/MyFinances/assets/69644118/cc49e3e7-d1c0-4907-a448-ff1a1c2051fa)

### Kalkulator Kredytowy
![image](https://github.com/jikoso2/MyFinances/assets/69644118/4d3f37ff-c586-42b2-856f-e8a7a8a63022)

Kalkulator ten umożliwia kalkulacje kredytu, świetnie wizualizuje przebieg spłaty całego kredytu.

![image](https://github.com/jikoso2/MyFinances/assets/69644118/333e9287-ad3b-4365-9397-814a70591bfa)

Pozwala również na wprowadzenie zmiennego oprocentowania oraz nadpłaty kredytu - wizualizując zmiany w kredycie oraz zmiany w ratach

![image](https://github.com/jikoso2/MyFinances/assets/69644118/707bef42-0e09-4f5b-a573-7ea75c9109f9)
![image](https://github.com/jikoso2/MyFinances/assets/69644118/801fe771-a0e0-4865-9f2d-0b8504573c04)

Dodatkowo dla zalogowanych użytkowników można zapisywać i szybko wczytywać różne konfiguracje kredytów.

![image](https://github.com/jikoso2/MyFinances/assets/69644118/be343a1f-9788-4590-8354-c83e5783814d)

### Kalkulator Lokat

![image](https://github.com/jikoso2/MyFinances/assets/69644118/c882bf00-201c-45ee-82b1-c613afacfede)

Jeden z najpopularnijszych sposobów oszczędzania wśród Polaków, kalkulator pomaga przewidzieć zyski z lokat.

![image](https://github.com/jikoso2/MyFinances/assets/69644118/7c67a169-3d29-4549-b567-d7cdead4cbbf)

### Kalkulator Konta Oszczędnościowego
![image](https://github.com/jikoso2/MyFinances/assets/69644118/097b1868-ee43-4a95-a386-d6b89898dc9f)

Równie popularny środek oszczędzania jak lokaty. Póki co istnieje podstawowa wersja tego kalkulatora, wkrótce zostanie rozwinięta.

![image](https://github.com/jikoso2/MyFinances/assets/69644118/106bbc8b-cc22-4d13-96c3-52eb0b4758e5)

### Kalkulator PPK
![image](https://github.com/jikoso2/MyFinances/assets/69644118/28fa706f-2708-4946-8428-2f4b87c619d2)

Jeden z bardziej skomplikowanych kalkulatorów, pozwala na obliczenia dotyczące Pracowniczych Planów Kapitałowych, przewidywania zastępywalności ostatniej pensji przez emerytury z ZUSu wynoszą marne 25-30% dla obecnych 30-latków, warto poczynić odpowiednie kroki aby zabezpieczyć swoją przyszłość.

![image](https://github.com/jikoso2/MyFinances/assets/69644118/83aa45b0-ed87-423f-ab09-da6620457e2b)

### Kalkulator wypłat PPK
![image](https://github.com/jikoso2/MyFinances/assets/69644118/f74b1639-2aee-4926-a5c0-a1ac0e8345f3)

Kalkulator obliczający wypłate po zerwaniu PPK przed 60 rokiem życia. Nawet wypłacanie co miesięczne pełnej kwoty z PPK jest opłacalne dla uczestnika tego programu.

![image](https://github.com/jikoso2/MyFinances/assets/69644118/061b96f8-14e0-4148-8111-0d99451affd3)

### Kontakt
![image](https://github.com/jikoso2/MyFinances/assets/69644118/61446fb6-ca9b-43a9-9ea2-3cadd9675233)

Formularz ten pozwala na wiadomości e-mail autorowi.

![image](https://github.com/jikoso2/MyFinances/assets/69644118/cd17d5be-ece2-46ad-b3a1-cf0dd2f668d3)
![image](https://github.com/jikoso2/MyFinances/assets/69644118/9acb662b-6222-47f2-b9b0-d1f46cff9eb5)

### Wersja mobilna
Istnieje możliwość korzystania z aplikacji również na urządzeniach mobilnych.

<img src="./img_README/mobile_1.jpg" width="250" height="500"> <img src="./img_README/mobile_2.jpg" width="250" height="500"> <img src="./img_README/mobile_3.jpg" width="250" height="500">

### Walidatory
Większość pól w aplikacji jest zabezpieczona przed wprowadzaniem złych wartości.
Jeśli jednak użytkownikowi uda się wprowadzić coś błędnie, dostanie o tym informacje.
##### Błędy w kalkulatorze kredytów
![Example Error1](./img_README/error_1.png)
##### Błędy w kalkulatorze obligacji
![Example Error2](./img_README/error_2.png)

## Testy
Z uwagi na dosyć złożone procesy obliczeniowe dotyczace lokat,kredytów czy obligacji stworzyłem podstawowy pakiet testów, który jest weryfikowany na konkretnych przykładach z odpowiednimi danymi wejściowymi.
Odpowiednie otestowanie kilku przykładowych scenariuszy obliczeń pozwoliło mi uzyskać pewność co do poprawności wyników, scenariusze przygotowywałem na podstawie swojej wiedzy oraz kalkulatora i arkusza excel.
Dodatkowo stworzyłem kilka testów sprawdzających poprawność ładowania się poszczególnych stron, zmian interfejsu użytkownika w zależności od wybranego kalkulatora.

![image](https://github.com/jikoso2/MyFinances/assets/69644118/3c8a9f38-ed50-4f4e-ba92-f5a1f257bc35)

![image](https://github.com/jikoso2/MyFinances/assets/69644118/04dd9bde-2898-4b82-9a96-bd71752c6e7e)

## Przykłady Kodu
Tabele podsumowywujące SummaryTable.razor
```
   <p>
    <div class="card-footer">
        @foreach (var item in Content)
        {
            <div class="row" id="rowinfoot">
                <div class="col-7">@item.Item1</div>
                <div class="col-5 text-right" style="white-space:nowrap">@item.Item2</div>
            </div>
        }
    </div>
</p>

@code {
    [Parameter]
    public List<Tuple<string, string>> Content { get; set; }
}
```

Inicjalizacja serwisów odpowiedzialnych za poszczególne zakładki
```
services.AddSingleton<DebentureService>();
services.AddSingleton<LoanService>();
services.AddSingleton<DepositService>();
services.AddSingleton<PPKService>();
services.AddSingleton<MailService>();
services.AddSingleton<PPKPayoutService>();
services.AddSingleton<DepositAccountService>();
```

## Baza Danych

Prosty schemat relacyjnej bazy danych w PostgreSQL, hosting bazy realizowany jest w serwisie AWS.

![image](https://github.com/jikoso2/MyFinances/assets/69644118/08a7bcda-7e2f-4f93-8e8e-bb5d4363adb2)

## Inspiracja

Moje Finanse została stworzona na potrzeby własnych prostych kalkulacji finansowych.
Aplikacja ta pozwoliła zastąpić excel i przyspieszyć wiele szacunków/obliczeń.
Rozpowszechniona została wśród moich znajomych, który bardzo pozytywnie ją odebrali.

## Kontakt
Stworzone przez Jarosław Czerniak [@jikoso2](https://github.com/jikoso2) - Odwiedź mój GitHub!
