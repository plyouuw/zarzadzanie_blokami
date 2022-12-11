using System.Text.Json;
using static Toolbox;

namespace zadanie
{
    internal class Program
    {
        struct Mieszkanie
        {
            public int numer_mieszkania;
            public int numer_bloku;
            public int dlugosc;
            public int szerokosc;
            public int powierzchnia;
            public int cena_za_m2;
            public int cena_calkowita;
            public int dostepnosc;

            public Mieszkanie()
            {
                numer_mieszkania = -1;
                numer_bloku = -1;
                dlugosc = -1;
                szerokosc = -1;
                powierzchnia = -1;
                cena_za_m2 = -1;
                cena_calkowita = -1;
                dostepnosc = -1;
            }
            public Dictionary<string, int> ZamienNaSlownik()
            {
                Dictionary<string, int> output = new()
                {
                    { "numer_mieszkania", numer_mieszkania },
                    { "dlugosc", dlugosc },
                    { "szerokosc", szerokosc },
                    { "cena_za_m2", cena_za_m2 },
                    { "dostepnosc", dostepnosc }
                };
                return output;
            }
        }
        struct Blok
        {
            public int numer_bloku;
            public Mieszkania mieszkania;

            public Blok()
            {
                this.numer_bloku = -1;
                this.mieszkania = new();
            }
        }
        
        class Bloki
        {
            public List<Blok> Lista;

            public Bloki()
            {
                Lista = new();
            }

            public List<int> NumeryBlokow()
            {
                List<int> numery = new();
                foreach (Blok b in Lista)
                    numery.Add(b.numer_bloku);
                numery.Sort();
                return numery;
            }
            public void UtworzBlok()
            {
                int numer_bloku = NajblizszyDostepnyNumerBloku();
                Blok blok;
                int ilosc_mieszkan = ReadInt(true, "\nWprowadź liczbę dostępnych mieszkań w bloku nr " + numer_bloku + ": ", "Liczba dostępnych mieszkań nie może być mniejsza od 1!", 1);
                blok.numer_bloku = numer_bloku;
                Mieszkania mieszkania = new();
                for (int i = 0; i < ilosc_mieszkan; i++)
                {
                    mieszkania.UtworzMieszkanie(numer_bloku, true);
                }
                blok.mieszkania = mieszkania;
                Lista.Add(blok);
                Sortuj();
            }
            public void DodajBlok(Blok blok)
            {
                Lista.Add(blok);
                Sortuj();
            }
            public bool ModyfikujBlok(Blok zmodyfikowany_blok)
            {
                Blok b = Wybierz(zmodyfikowany_blok.numer_bloku);
                if (b.numer_bloku == -1) return false;
                UsunBlok(zmodyfikowany_blok.numer_bloku);
                DodajBlok(zmodyfikowany_blok);
                return true;
            }
            public bool UsunBlok(int numer_bloku)
            {
                Blok b = Wybierz(numer_bloku);
                if (b.numer_bloku == -1) return false;
                Lista.Remove((Blok)b);
                return true;
            }
            public Blok Wybierz(int numer_bloku)
            {
                foreach (Blok b in Lista)
                    if (b.numer_bloku == numer_bloku) 
                        return b;
                return new();
            }
            public void WyswietlWszystkieMieszkania()
            {
                if (!Lista.Any())
                {
                    PrintError("Brak dodanych bloków.\n");
                    return;
                }
                Print("Wszystkie mieszkania: \n");
                foreach (Blok b in Lista)
                {
                    Print("\nBlok nr " + b.numer_bloku + ":\n");
                    if (!b.mieszkania.Lista.Any()) PrintError("Brak dodanych mieszkań.\n");
                    else
                    {
                        foreach (Mieszkanie m in b.mieszkania.Lista)
                        {
                            Print("\tNumer mieszkania: " + m.numer_mieszkania + "\n");
                            Print("\tDługość: " + m.dlugosc + "m\n");
                            Print("\tSzerokość: " + m.szerokosc + "m\n");
                            Print("\tPowierzchnia: " + m.powierzchnia + "m2\n");
                            Print("\tCena za m2: " + m.cena_za_m2 + "zł\n");
                            Print("\tCena całkowita: " + m.cena_calkowita + "zł\n");
                            Print("\tDostępne: " + (m.dostepnosc == 1 ? "tak" : "nie") + "\n\n");
                        }
                    }
                }
            }
            public void ZnajdzMieszkanie()
            {
                Print("Wyszukiwarka mieszkań\n");
                int metraz = ReadInt(true, "Podaj poszukiwany metraż mieszkania: ", "Metraż mieszkania nie może być mniejszy od 1!", 1);
                Mieszkanie znalezione_mieszkanie = Szukaj(metraz);
                if (znalezione_mieszkanie.numer_mieszkania == -1)
                {
                    PrintError("\nNie znaleziono mieszkania pasującego do podanego metrażu!");
                    return;
                }
                string roznica = "";
                if (znalezione_mieszkanie.powierzchnia != metraz)
                {
                    if (metraz > znalezione_mieszkanie.powierzchnia) roznica = " (jest mniejsze od szukanego o " + (metraz - znalezione_mieszkanie.powierzchnia) + "m2)";
                    else roznica = " (jest większe od szukanego o " + (znalezione_mieszkanie.powierzchnia - metraz) + "m2)";
                }
                Print("\nZnaleziono mieszaknie!\n" +
                    "Blok nr " + znalezione_mieszkanie.numer_bloku + " mieszkanie " + znalezione_mieszkanie.numer_mieszkania + "\n" +
                    "Metraż - " + znalezione_mieszkanie.powierzchnia + "m2" + roznica + "\n" +
                    "Cena - " + znalezione_mieszkanie.cena_calkowita + "zł");

            }
            private Mieszkanie Szukaj(int metraz, int zakres_bledu = 7)
            {
                List<Mieszkanie> zgodne_mieszkania = new();
                List<int> ceny_do_sortowania = new();

                foreach (Blok b in Lista)
                {
                    foreach (Mieszkanie m in b.mieszkania.Lista)
                    {
                        if (m.powierzchnia <= metraz + zakres_bledu && m.powierzchnia >= metraz - zakres_bledu && m.dostepnosc == 1)
                        {
                            zgodne_mieszkania.Add(m);
                            ceny_do_sortowania.Add(m.cena_za_m2 * m.powierzchnia);
                        }
                    }
                }

                ceny_do_sortowania.Sort();

                foreach (Mieszkanie m in zgodne_mieszkania)
                {
                    if (m.cena_calkowita == ceny_do_sortowania.First()) return m;
                }

                return new Mieszkanie();
            }
            private int NajblizszyDostepnyNumerBloku()
            {
                for (int x = 1; x < NumeryBlokow().Count+1; x++)
                {
                    if (!NumeryBlokow().Contains(x))
                    {
                        return x;
                    }
                }
                return NumeryBlokow().Count+1;
            }
            private void Sortuj()
            {
                List<Blok> posortowane = new();
                List<int> numery = NumeryBlokow();
                for (int x = numery.First(); x <= numery.Last(); x++)
                {
                    foreach (Blok b in Lista)
                    {
                        if (b.numer_bloku == x) posortowane.Add(b);
                    }
                }
                Lista = posortowane;
            }
        }
        class Mieszkania
        {
            public List<Mieszkanie> Lista;

            public Mieszkania()
            {
                Lista = new(); 
            }

            public void UtworzMieszkanie(int numer_bloku, bool dostepnosc)
            {
                int numer_mieszkania = NajblizszyDostepnyNumerMieszkania();
                Mieszkanie mieszkanie = new() { numer_mieszkania = numer_mieszkania, numer_bloku = numer_bloku, dostepnosc = dostepnosc?1:0 };

                int dlugosc = ReadInt(true, "\nWprowadź długość mieszkania nr " + numer_mieszkania + " w bloku nr " + numer_bloku + ": ", "Długość mieszkania nie może być mniejsza od 1 i większa od 15!", 1, 15); ;
                mieszkanie.dlugosc = dlugosc;

                int szerokosc = ReadInt(true, "Wprowadź szerokość mieszkania nr " + numer_mieszkania + " w bloku nr " + numer_bloku + ": ", "Szerokość mieszkania nie może być mniejsza od 1 i większa od 15!", 1, 15);
                mieszkanie.szerokosc = szerokosc;

                mieszkanie.powierzchnia = dlugosc * szerokosc;

                int max_cena_za_m2 = int.MaxValue / mieszkanie.powierzchnia / 100;
                int cena_za_m2 = ReadInt(true, "Wprowadź cenę za m2 mieszkania nr " + numer_mieszkania + " w bloku nr " + numer_bloku + ": ", "Cena za m2 mieszkania nie może być mniejsza od 1 i większa od " + max_cena_za_m2 + "!", 1, max_cena_za_m2);
                mieszkanie.cena_za_m2 = cena_za_m2;

                mieszkanie.cena_calkowita = dlugosc * szerokosc * cena_za_m2;

                Lista.Add(mieszkanie);
                Sortuj();
            }
            public void DodajMieszkanie(Mieszkanie mieszkanie)
            {
                Lista.Add(mieszkanie);
                Sortuj();
            }
            public bool ModyfikujMieszkanie(Mieszkanie zmodyfikowane_mieszkanie)
            {
                Mieszkanie m = Wybierz(zmodyfikowane_mieszkanie.numer_mieszkania);
                if (m.numer_mieszkania == -1) return false;
                UsunMieszkanie(zmodyfikowane_mieszkanie.numer_mieszkania);
                DodajMieszkanie(zmodyfikowane_mieszkanie);
                return true;
            }
            public bool UsunMieszkanie(int numer_mieszkania)
            {
                Mieszkanie m = Wybierz(numer_mieszkania);

                if (m.numer_mieszkania == -1) return false;

                Lista.Remove((Mieszkanie)m);
                return true;
            }
            public List<int> NumeryMieszkan()
            {
                List<int> numery = new();
                foreach (Mieszkanie m in Lista)
                    numery.Add(m.numer_mieszkania);
                numery.Sort();
                return numery;
            }
            public Mieszkanie Wybierz(int numer_mieszkania)
            {
                foreach (Mieszkanie m in Lista)
                    if (m.numer_mieszkania == numer_mieszkania)
                        return m;
                return new();
            }
            public bool ZmienDostepnosc(bool dostepnosc, int numer_mieszkania)
            {
                Mieszkanie m = Wybierz(numer_mieszkania);
                if (m.numer_mieszkania == -1) return false;
                m.dostepnosc = dostepnosc ? 1 : 0;
                UsunMieszkanie(numer_mieszkania);
                Lista.Add(m);
                Sortuj();
                return true;
            }
            private int NajblizszyDostepnyNumerMieszkania()
            {
                for(int x = 1; x < NumeryMieszkan().Count+1; x++)
                {
                    if(!NumeryMieszkan().Contains(x))
                    {
                        return x;
                    } 
                }
                return NumeryMieszkan().Count + 1;
            }
            private void Sortuj()
            {
                List<Mieszkanie> posortowane = new();
                List<int> numery = NumeryMieszkan();
                for (int x = numery.First(); x <= numery.Last(); x++)
                    foreach (Mieszkanie m in Lista)
                        if (m.numer_mieszkania == x) posortowane.Add(m);

                Lista = posortowane;
            }
        }

        static Bloki Init(Menu.Theme styl_menu)
        {
            Bloki bloki = new();
            Dictionary<int, string> lista_opcji = new() {
                { 1, "Wprowadź nową listę bloków" },
                { 2, "Wczytaj listę bloków i mieszkań z pliku" }
            };
            Menu menu = new(styl_menu, lista_opcji, "Co chcesz zrobić?");
            int selected = menu.ReadChoice();

            Console.Clear();

            if (selected == 2)
            {
                Print("\nPodaj ścieżkę do pliku, w którym chcesz zapisać dane: ");
                string[]? wczytany_tekst = WczytajPlik(Read());
                if (wczytany_tekst != null)
                {
                    Bloki? wczytane_bloki = JsonDoListyBlokow(string.Concat(wczytany_tekst));
                    if (wczytane_bloki != null)
                    {
                        bloki = wczytane_bloki;
                        bloki.WyswietlWszystkieMieszkania();
                    }
                    else PrintError("wczytane_bloki -> null");
                    Czekaj();
                } else Czekaj();
                return bloki;
            }
            Print("Wprowadzanie danych \n");
            int ilosc_blokow = ReadInt(true, "Wprowadź maksymalną liczbę bloków: ", "Liczba bloków nie może być mniejsza od 1!", 1);

            for (int i = 0; i < ilosc_blokow; i++)
                bloki.UtworzBlok();

            Console.Clear();

            return bloki;
        }

        static bool ZapiszDoPliku(string nazwa_pliku, Bloki bloki) => ZapiszPlik(nazwa_pliku, ListaBlokowDoJson(bloki));
        static void WczytajZPliku(string nazwa_pliku, out Bloki bloki)
        {
            string[]? wczytany_tekst = WczytajPlik(nazwa_pliku);
            if (wczytany_tekst != null)
            {
                Bloki? wczytane_bloki = JsonDoListyBlokow(string.Concat(wczytany_tekst));
                if (wczytane_bloki != null)
                {
                    bloki = wczytane_bloki;
                    wczytane_bloki.WyswietlWszystkieMieszkania();
                }
                else
                {
                    PrintError("Wystąpił błąd w trakcie wczytywania listy bloków!");
                    bloki = new();
                }
            }
            else bloki = new();
        }
        static void UtworzMieszkanie(ref Bloki bloki)
        {
            Dictionary<int, string> lista = new();
            foreach(Blok b in bloki.Lista)
            {
                lista.Add(b.numer_bloku, $"Ilość mieszkań: {b.mieszkania.Lista.Count}");
            }
            Menu menu = new(Menu.Theme.WhiteBacklight, lista, "Wybierz numer bloku, do którego chcesz dodać nowe mieszkanie: ");

            Blok blok = bloki.Wybierz(menu.ReadChoice());
            if (blok.numer_bloku == -1)
            {
                PrintError("\nWystąpił nieoczekiwany błąd podczas wyboru bloku!");
                return;
            }
            else
            {
                blok.mieszkania.UtworzMieszkanie(blok.numer_bloku, true);
                Print("\nZakończono dodawanie nowego mieszkania!");
                return;
            }
        }
        static void UsunBlok(ref Bloki bloki)
        {
            Dictionary<int, string> lista = new();
            foreach (Blok b in bloki.Lista)
            {
                lista.Add(b.numer_bloku, $"Ilość mieszkań: {b.mieszkania.Lista.Count}");
            }
            Menu menu = new(Menu.Theme.WhiteBacklight, lista, "Wybierz numer bloku do usunięcia: ");

            int numer_bloku = menu.ReadChoice();
            if (!bloki.UsunBlok(numer_bloku)) PrintError("\nWystąpił nieoczekiwany błąd podczas wyboru bloku!");
            else Print("\nUsunięto blok o numerze " + numer_bloku + "!");
        }
        static void UsunMieszkanie(ref Bloki bloki)
        {
            Dictionary<int, string> lista = new();
            foreach (Blok b in bloki.Lista)
            {
                lista.Add(b.numer_bloku, $"Ilość mieszkań: {b.mieszkania.Lista.Count}");
            }
            Menu menu = new(Menu.Theme.WhiteBacklight, lista, "Wybierz blok, z którego chcesz usunąć mieszkanie: ");

            int numer_bloku = menu.ReadChoice();
            Blok blok = bloki.Wybierz(numer_bloku);
            if (blok.numer_bloku == -1)
            {
                PrintError("Nie znaleziono bloku o takim numerze!");
                return;
            }

            lista.Clear();
            foreach (Mieszkanie m in blok.mieszkania.Lista)
            {
                lista.Add(m.numer_mieszkania, $"{m.dlugosc}m x {m.szerokosc}m, {m.powierzchnia}m2");
            }
            menu = new(Menu.Theme.WhiteBacklight, lista, "Wybierz mieszkanie do usunięcia: ");

            int numer_mieszkania = menu.ReadChoice();
            if (!blok.mieszkania.UsunMieszkanie(numer_mieszkania))
            {
                PrintError("Nie znaleziono mieszkania o takim numerze!");
                return;
            }
            blok.mieszkania.UsunMieszkanie(numer_mieszkania);
            if (!bloki.ModyfikujBlok(blok))
            {
                PrintError("Nie znaleziono mieskzania o takim numerze!");
                return;
            }
            Print("Usunięto mieszkanie nr " + numer_mieszkania + " z bloku o numerze " + numer_bloku + "!");
        }
        static void ZmienDostepnosc(ref Bloki bloki)
        {
            Dictionary<int, string> lista = new();
            foreach (Blok b in bloki.Lista)
            {
                lista.Add(b.numer_bloku, $"Ilość mieszkań: {b.mieszkania.Lista.Count}");
            }
            Menu menu = new(Menu.Theme.WhiteBacklight, lista, "Wybierz blok, z którego chcesz usunąć mieszkanie: ");

            int numer_bloku = menu.ReadChoice();
            Blok aktualny_blok = bloki.Wybierz(numer_bloku);
            if (aktualny_blok.numer_bloku == -1)
            {
                PrintError("Nie znaleziono bloku o takim numerze!");
                return;
            }

            lista.Clear();
            foreach (Mieszkanie m in aktualny_blok.mieszkania.Lista)
            {
                lista.Add(m.numer_mieszkania, $"{m.dlugosc}m x {m.szerokosc}m, {m.powierzchnia}m2");
            }
            menu = new(Menu.Theme.WhiteBacklight, lista, "Wybierz mieszkanie do usunięcia: ");

            int numer_mieszkania = menu.ReadChoice();
            Mieszkanie mieszkanie = aktualny_blok.mieszkania.Wybierz(numer_mieszkania);
            if (mieszkanie.numer_mieszkania == -1)
            {
                PrintError("Nie znaleziono mieszkania o takim numerze!");
                return;
            }
            lista.Clear();
            lista.Add(1, "Dostępne");
            lista.Add(2, "Zajęte");
            menu = new(Menu.Theme.WhiteBacklight, lista, "Jaki status ustawić?");
            bool status_dostepnosci = menu.ReadChoice() == 1;
            if (!aktualny_blok.mieszkania.ZmienDostepnosc(status_dostepnosci, numer_mieszkania))
            {
                PrintError("Wystąpił nieoczekiwany błąd.\n");
                return;
            }
            Print("Zmieniono dostępność mieszkania nr " + numer_mieszkania + " w bloku nr " + numer_bloku + " na " + (status_dostepnosci ? "dostępne" : "zajęte"));

        }

        static string ListaBlokowDoJson(Bloki bloki)
        {
            Dictionary<int, List<Dictionary<string, int>>> dict = new();
            foreach (Blok b in bloki.Lista)
            {
                List<Dictionary<string, int>> list = new();
                foreach(Mieszkanie m in b.mieszkania.Lista)
                {
                    list.Add(m.ZamienNaSlownik());
                }
                dict.Add(b.numer_bloku, list);
            }
            string json = JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true });
            return json;
        }
        static Bloki? JsonDoListyBlokow(string? bloki_json)
        {
            if (bloki_json == null) return null;
            Dictionary<int, List<Dictionary<string, int>>>? dict = JsonSerializer.Deserialize<Dictionary<int, List<Dictionary<string, int>>>>(bloki_json);
            if (dict == null) return null; 

            Bloki bloki = new();
            Blok wczytany_blok;
            Mieszkania mieszkania;
            Mieszkanie wczytane_mieszkanie;
            foreach (var blok in dict)
            {
                wczytany_blok = new();
                mieszkania = new();
                foreach(var mieszkania_dict in blok.Value)
                {
                    wczytane_mieszkanie = new();
                    foreach (var mieszkanie in mieszkania_dict)
                    {
                        switch (mieszkanie.Key)
                        {
                            case "numer_mieszkania":
                                wczytane_mieszkanie.numer_mieszkania = mieszkanie.Value;
                                break;
                            case "dlugosc":
                                wczytane_mieszkanie.dlugosc = mieszkanie.Value;
                                break;
                            case "szerokosc":
                                wczytane_mieszkanie.szerokosc = mieszkanie.Value;
                                break;
                            case "cena_za_m2":
                                wczytane_mieszkanie.cena_za_m2 = mieszkanie.Value;
                                break;
                            case "dostepnosc":
                                wczytane_mieszkanie.dostepnosc = mieszkanie.Value;
                                break;
                        }
                    }
                    wczytane_mieszkanie.powierzchnia = wczytane_mieszkanie.dlugosc * wczytane_mieszkanie.szerokosc;
                    wczytane_mieszkanie.numer_bloku = blok.Key;
                    wczytane_mieszkanie.cena_calkowita = wczytane_mieszkanie.cena_za_m2 * wczytane_mieszkanie.powierzchnia;
                    mieszkania.Lista.Add(wczytane_mieszkanie);
                }
                wczytany_blok.numer_bloku = blok.Key;
                wczytany_blok.mieszkania = mieszkania;
                bloki.Lista.Add(wczytany_blok);
            }
            return bloki.Lista.Count < 0 ? null : bloki;
        }
        static void GlownaPetla(Bloki bloki, Menu menu)
        {
            string nazwa_pliku;
            int input;
            while ((input = menu.ReadChoice()) > 0)
            {
                Console.Clear();

                if (input == 0) return;
                else if (input == 1) bloki.ZnajdzMieszkanie();
                else if (input == 2) bloki.WyswietlWszystkieMieszkania();
                else if (input == 3) { bloki.UtworzBlok(); Print("\nZakończono dodawanie nowego bloku!"); }
                else if (input == 4) UtworzMieszkanie(ref bloki);
                else if (input == 5) UsunBlok(ref bloki);
                else if (input == 6) UsunMieszkanie(ref bloki);
                else if (input == 7) ZmienDostepnosc(ref bloki);
                else if (input == 8)
                {
                    Print("\nPodaj ścieżkę do pliku, w którym chcesz zapisać dane: ");
                    nazwa_pliku = Read();
                    if (ZapiszDoPliku(nazwa_pliku, bloki)) Print("Pomyślnie zapisano dane!");
                }
                else if (input == 9)
                {
                    Print("\nPodaj ścieżkę do pliku, gdzie są zapisane dane: ");
                    nazwa_pliku = Read();
                    WczytajZPliku(nazwa_pliku, out bloki);
                }
                else PrintError("Nieznany błąd! Spróbuj ponownie.");
                
                Czekaj();
            }
        }

        static void Main()
        {
            Menu.Theme styl_menu = Menu.Theme.WhiteBacklight;
            Bloki bloki = Init(styl_menu);

            Dictionary<int, string> lista_opcji = new()
            {
                { 1, "Znajdź mieszkanie" },
                { 2, "Wyświetl wszystkie mieszkania" },
                { 3, "Dodaj nowy blok" },
                { 4, "Dodaj nowe mieszkanie" },
                { 5, "Usuń istniejący blok" },
                { 6, "Usuń istniejące mieszkanie" },
                { 7, "Zmień dostępność istniejącego mieszkania" },
                { 8, "Zapisz listę bloków i mieszkań do pliku" },
                { 9, "Wczytaj listę bloków i mieszkań z pliku" },
                { 0, "Zakończ działanie programu" }
            };
            Menu menu = new(styl_menu, lista_opcji, "Lista możliwych operacji:");

            GlownaPetla(bloki, menu);
        }
    }
}