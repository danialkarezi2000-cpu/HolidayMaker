using MySqlConnector;
string password = File.ReadAllText("dbpassword.txt").Trim();
string connectionString =
$"server=localhost;Database=holiday_maker;User Id=root;Password={password};UseAffectedRows=false;";
await using var connection = new MySqlConnection(connectionString);
await connection.OpenAsync();
Console.WriteLine("Koppla till databasen!");
await using var command = new MySqlCommand("SELECT id,name FROM cities ORDER BY id;", connection);
await using var reader = await command.ExecuteReaderAsync();
while (await reader.ReadAsync())
{
    Console.WriteLine($"{reader.GetInt64("id")}: {reader.GetString("name")}");
}
