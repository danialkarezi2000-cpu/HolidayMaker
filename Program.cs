using MySqlConnector;
string password = File.ReadAllText("dbpassword.txt").Trim();
string connectionString =
$"server=localhost;Database=holiday_maker;User Id=root;Password={password};UseAffectedRows=false;";
await using var connection = new MySqlConnection(connectionString);
await connection.OpenAsync();
var customerRepository = new CustomerRepository(connection);
var roomRepository = new RoomRepository(connection);
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

            var rooms = await roomRepository.SearchAvailableRoomsAsync(city, start, end, persons);
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
            Console.WriteLine("boka rum kommer snart");
            break;
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