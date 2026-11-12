public static class Menu
{
    public static void ShowMainMenu()
    {
        Console.WriteLine();
        Console.WriteLine("==HOLIDAY MAKER==");
        Console.WriteLine("1. Registrera kund");
        Console.WriteLine("2. Sök lediga rum");
        Console.WriteLine("3. Boka rum");
        Console.WriteLine("4. Visa mina bokningar");
        Console.WriteLine("5. Ändra antal personer");
        Console.WriteLine("6. Avboka");
        Console.WriteLine("7. Logga in med e-post");
        Console.WriteLine("0. Avsluta");
        Console.WriteLine("Välj: ");
    }

    // Frågar tills användaren skriver något som inte är tomt
    public static string ReadRequired(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine()?.Trim() ?? "";
            if (input != "")
            {
                return input;
            }
            Console.WriteLine("Fältet får inte vara tomt, försök igen.");
        }
    }

    // Fråga tills användaren skriver ett heltal.
    public static int ReadInt(string prompt, int min)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int value) && value >= min)
            {
                return value;
            }
            Console.WriteLine($"Skriv ett heltal som är minst {min}.");
        }
    }

    // Fråga tills användaren skriver ett datum i formatet.
    public static DateTime ReadDate(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (DateTime.TryParseExact(Console.ReadLine()?.Trim(), "yyyy-MM-dd", null,
                System.Globalization.DateTimeStyles.None, out DateTime date))
            {
                return date;
            }
            Console.WriteLine("Fel format. Skriv till exempel 2026-12-03.");
        }
    }

    // frågar j/n och returnerar true om svaret är j
    public static bool ReadYesNo(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string answer = Console.ReadLine()?.Trim().ToLower() ?? "";
            if (answer == "j") return true;
            if (answer == "n") return false;
            Console.WriteLine("Svara j eller n.");
        }
    }

    // skriver ut hotellen som en tabell, så att man ser vilka städer man kan välja
    public static void ShowHotelTable(List<HotelInfo> hotels)
    {
        Console.WriteLine();
        Console.WriteLine($"{"Stad",-9}{"Hotell",-19}{"Pris",8}{"Strand",8}{"Centrum",8}{"Pool",5}{"Rest",5}{"Betyg",6}");
        foreach (var h in hotels)
        {
            Console.WriteLine($"{h.CityName,-9}{h.HotelName,-19}{h.MinPrice,5:0} kr{h.Beach,6} m{h.Center,6} m" +
                              $"{(h.Pool ? "ja" : "nej"),5}{(h.Restaurant ? "ja" : "nej"),5}{h.Rating,6}");
        }
        Console.WriteLine();
    }
}
