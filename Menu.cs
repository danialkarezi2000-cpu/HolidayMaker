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
}
