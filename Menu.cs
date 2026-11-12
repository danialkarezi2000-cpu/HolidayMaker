// Ett eget "fel" som betyder: användaren vill gå tillbaka till menyn
public class GoBackException : Exception { }

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
        Console.WriteLine("(Skriv x när som helst för att gå tillbaka hit)");
        Console.WriteLine("Välj: ");
    }

    // Läser en rad. Om man skriver x går man tillbaka till menyn.
    private static string Read()
    {
        string input = Console.ReadLine()?.Trim() ?? "";
        if (input.ToLower() == "x")
        {
            throw new GoBackException();
        }
        return input;
    }

    // Frågar tills användaren skriver något som inte är tomt
    public static string ReadRequired(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Read();
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
            if (int.TryParse(Read(), out int value) && value >= min)
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
            if (DateTime.TryParseExact(Read(), "yyyy-MM-dd", null,
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
            string answer = Read().ToLower();
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

    // skriver ut de lediga rummen som en tabell
    public static void ShowRoomTable(List<AvailableRoom> rooms)
    {
        Console.WriteLine();
        Console.WriteLine($"{"Id",-4}{"Hotell",-19}{"Stad",-9}{"Rum",-6}{"Pers",5}{"Pris",9}{"Betyg",6}");
        foreach (var r in rooms)
        {
            Console.WriteLine($"{r.RoomId,-4}{r.HotelName,-19}{r.CityName,-9}{r.RoomName,-6}" +
                              $"{r.Capacity,5}{r.Price,6:0} kr{r.Rating,6}");
        }
        Console.WriteLine();
    }

    // skriver ut kundens bokningar som en tabell
    public static void ShowBookingTable(List<BookingInfo> bookings)
    {
        Console.WriteLine();
        Console.WriteLine($"{"Nr",-4}{"Hotell",-19}{"Rum",-6}{"Från",-11}{"Till",-11}{"Pers",5}{"Totalt",10}");
        foreach (var b in bookings)
        {
            Console.WriteLine($"{b.BookingId,-4}{b.HotelName,-19}{b.RoomName,-6}" +
                              $"{b.Start,-11:yyyy-MM-dd}{b.End,-11:yyyy-MM-dd}{b.Persons,5}{b.TotalPrice,7:0} kr");
        }
        Console.WriteLine();
    }
}
