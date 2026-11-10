using MySqlConnector;
public class CustomerRepository(MySqlConnection connection)
{
    //method som sparar en kund i databasen och returnerar det nya kundens id
    public async Task<long> CreateCustomerAsync(string name, string lastname, string email, string tlf)
    {
        await using var command = new MySqlCommand(
            "INSERT INTO customers (name, last_name, email, tlf) VALUES (@name, @lastname, @email, @tlf);", connection);
        command.Parameters.AddWithValue("@name", name);
        command.Parameters.AddWithValue("@lastname", lastname);
        command.Parameters.AddWithValue("@email", email);
        command.Parameters.AddWithValue("@tlf", tlf);
        await command.PrepareAsync();
        await command.ExecuteNonQueryAsync();
        return command.LastInsertedId;
    }

    // method som hämtar kundens id baserat på e-post
    public async Task<long?> GetCustomerIdByEmailAsync(string email)
    {
        await using var command = new MySqlCommand(
            "SELECT id FROM customers WHERE email = @email;", connection);
        command.Parameters.AddWithValue("@email", email);

        await command.PrepareAsync();
        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return reader.GetInt64("id");
        }
        return null;
    }
}
