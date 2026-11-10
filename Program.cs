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
            foreach (var room in rooms)
            {
                Console.WriteLine($"Rum-id {room.RoomId}: {room.HotelName} ({room.CityName}), rum {room.RoomName}, " +
              $"{room.Capacity} pers, {room.Price} kr/natt, betyg {room.Rating}");
            }
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
            Console.WriteLine("Visa bokningar");
            break;
        case "5":
            Console.WriteLine("byta antal personer");
            break;
        case "6":
            Console.WriteLine("Avboka min bokning");
            break;
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