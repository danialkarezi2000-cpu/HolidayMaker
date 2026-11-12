using MySqlConnector;
string password = File.ReadAllText("dbpassword.txt").Trim();
string connectionString =
$"server=localhost;Database=holiday_maker;User Id=root;Password={password};UseAffectedRows=false;";
await using var connection = new MySqlConnection(connectionString);
await connection.OpenAsync();
var customerRepository = new CustomerRepository(connection);
var roomRepository = new RoomRepository(connection);
var bookingRepository = new BookingRepository(connection);
long? loggedInCustomerId = null; // null = ingen är inloggad än
Console.WriteLine("Koppla till databasen!");
bool running = true;
while (running)
{
    Menu.ShowMainMenu();
    string? choice = Console.ReadLine()?.Trim();
    try
    {
    switch (choice)
    {
        case "1":
            string name = Menu.ReadRequired("Förnamn: ");
            string lastname = Menu.ReadRequired("Efternamn: ");
            string email = Menu.ReadRequired("E-post: ");
            string tlf = Menu.ReadRequired("Telefon: ");

            try
            {
                long newId = await customerRepository.CreateCustomerAsync(name, lastname, email, tlf);
                Console.WriteLine($"Kunden är registrerad med id {newId}");
                loggedInCustomerId = newId;
            }
            catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
            {
                Console.WriteLine("Den e-posten finns redan. Försök med en annan.");
            }
            break;
        case "2":
            var hotelList = await roomRepository.GetHotelOverviewAsync();
            Menu.ShowHotelTable(hotelList);
            string city = Menu.ReadRequired("Stad: ");
            DateTime start = Menu.ReadDate("Incheckning (ÅÅÅÅ-MM-DD): ");
            DateTime end = Menu.ReadDate("Utcheckning (ÅÅÅÅ-MM-DD): ");
            if (end <= start)
            {
                Console.WriteLine("Utcheckning måste vara efter incheckning.");
                break;
            }
            int persons = Menu.ReadInt("Antal personer: ", 1);

            bool needPool = Menu.ReadYesNo("Måste hotellet ha pool? (j/n): ");
            bool needRestaurant = Menu.ReadYesNo("Måste hotellet ha restaurang? (j/n): ");
            int maxBeach = Menu.ReadInt("Max meter till stranden (0 = spelar ingen roll): ", 0);
            int maxCenter = Menu.ReadInt("Max meter till centrum (0 = spelar ingen roll): ", 0);
            bool sortByRating = Menu.ReadYesNo("Sortera på betyg istället för pris? (j/n): ");
            var rooms = await roomRepository.SearchAvailableRoomsAsync(
                city, start, end, persons, needPool, needRestaurant, maxBeach, maxCenter, sortByRating);
            if (rooms.Count == 0)
            {
                Console.WriteLine("Inga lediga rum hittades.");
            }
            Menu.ShowRoomTable(rooms);
            break;
        case "3":
        {
            if (loggedInCustomerId == null)
            {
                Console.WriteLine("Du måste logga in först (val 7) eller registrera dig (val 1).");
                break;
            }
            int roomId = Menu.ReadInt("Rum-id (från sökningen): ", 1);
            DateTime checkIn = Menu.ReadDate("Incheckning (ÅÅÅÅ-MM-DD): ");
            DateTime checkOut = Menu.ReadDate("Utcheckning (ÅÅÅÅ-MM-DD): ");
            if (checkOut <= checkIn)
            {
                Console.WriteLine("Utcheckning måste vara efter incheckning.");
                break;
            }
            int guests = Menu.ReadInt("Antal personer: ", 1);
            try
            {
                var (bookingId, totalPrice) = await bookingRepository.CreateBookingAsync(
                    loggedInCustomerId.Value, roomId, checkIn, checkOut, guests);
                Console.WriteLine($"Bokat! Bokningsnummer {bookingId}. Totalpris {totalPrice} kr.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
            }
            break;
        }
        case "4":
        {
            if (loggedInCustomerId == null) { Console.WriteLine("Logga in först (val 7)."); break; }
            var myBookings = await bookingRepository.GetBookingsForCustomerAsync(loggedInCustomerId.Value);
            if (myBookings.Count == 0) Console.WriteLine("Du har inga bokningar.");
            Menu.ShowBookingTable(myBookings);
            break;
        }
        case "5":
        {
            if (loggedInCustomerId == null) { Console.WriteLine("Logga in först (val 7)."); break; }
            int changeId = Menu.ReadInt("Bokningsnummer: ", 1);
            int newPersons = Menu.ReadInt("Nytt antal personer: ", 1);
            bool changed = await bookingRepository.UpdatePersonsAsync(changeId, loggedInCustomerId.Value, newPersons);
            Console.WriteLine(changed
                ? "Antal personer är ändrat."
                : "Gick inte: bokningen finns inte, är inte din, eller så får inte så många plats i rummet.");
            break;
        }
        case "6":
        {
            if (loggedInCustomerId == null) { Console.WriteLine("Logga in först (val 7)."); break; }
            int cancelId = Menu.ReadInt("Bokningsnummer att avboka: ", 1);
            bool deleted = await bookingRepository.DeleteBookingAsync(cancelId, loggedInCustomerId.Value);
            Console.WriteLine(deleted ? "Bokningen är avbokad." : "Hittade ingen sådan bokning för dig.");
            break;
        }
        case "7":
            string loginEmail = Menu.ReadRequired("E-post: ");
            loggedInCustomerId = await customerRepository.GetCustomerIdByEmailAsync(loginEmail);
            if (loggedInCustomerId == null)
            {
                Console.WriteLine("Ingen kund med den e-posten. Registrera dig först (val 1).");
            }
            else
            {
                Console.WriteLine($"Inloggad som kund {loggedInCustomerId}");
            }
            break;
        case "0":
            running = false;
            break;
        default:
            Console.WriteLine("Felaktigt val försök igen");
            break;
    }
    }
    catch (GoBackException)
    {
        Console.WriteLine("Tillbaka till menyn.");
    }
}