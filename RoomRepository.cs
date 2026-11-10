using MySqlConnector;

public class RoomRepository(MySqlConnection connection)
{
    // method som hämtar alla tillgängliga rum baserat på stad, startdatum, slutdatum och antal personer.
    public async Task<List<AvailableRoom>> SearchAvailableRoomsAsync(
        string city, DateTime start, DateTime end, int persons)
    {
        await using var command = new MySqlCommand(
            """
            SELECT rooms.id, rooms.name, rooms.capacity, rooms.price,
                   hotels.name AS hotel_name, cities.name AS city_name, hotels.rating
            FROM rooms
            JOIN hotels ON rooms.hotel_id = hotels.id
            JOIN cities ON hotels.city_id = cities.id
            WHERE rooms.id NOT IN (
                SELECT room_id FROM bookings
                WHERE start_date < @end AND end_date > @start
            )
              AND rooms.capacity >= @persons
              AND cities.name = @city
            ORDER BY rooms.price;
            """, connection);
        command.Parameters.AddWithValue("@city", city);
        command.Parameters.AddWithValue("@start", start);
        command.Parameters.AddWithValue("@end", end);
        command.Parameters.AddWithValue("@persons", persons);
        await command.PrepareAsync();

        await using var reader = await command.ExecuteReaderAsync();
        var rooms = new List<AvailableRoom>();
        while (await reader.ReadAsync())
        {
            rooms.Add(new AvailableRoom(
                reader.GetInt64("id"), reader.GetString("name"), reader.GetInt32("capacity"),
                reader.GetDecimal("price"), reader.GetString("hotel_name"),
                reader.GetString("city_name"), reader.GetInt32("rating")));
        }
        return rooms;
    }
}
