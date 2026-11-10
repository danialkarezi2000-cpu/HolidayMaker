using MySqlConnector;
string password = File.ReadAllText("dbpassword.txt").Trim();
string connectionString =
$"server=localhost;Database=holiday_maker;User Id=root;Password={password};UseAffectedRows=false;";
await using var connection = new MySqlConnection(connectionString);
await connection.OpenAsync();
Console.WriteLine("Koppla till databasen!");
bool running = true;
while (running)
{
    Menu.ShowMainMenu();
    string? choice = Console.ReadLine()?.Trim();
    switch (choice)
    {
        case "1":
            Console.WriteLine("Registerera kund som kommer snart");
            break;
        case "2":
            Console.WriteLine("Söker lediga rum kommer snart");
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
        case "0":
            running = false;
            break;
        default:
            Console.WriteLine("Felaktigt val försök igen");
            break;
    }
}