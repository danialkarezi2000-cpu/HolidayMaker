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
}
