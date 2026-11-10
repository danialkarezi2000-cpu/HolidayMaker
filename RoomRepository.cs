using MySqlConnector;

public class RoomRepository(MySqlConnection connection)
{
    // method som hämtar alla tillgängliga rum baserat på stad, startdatum, slutdatum och antal personer.
    public async Task<List<AvailableRoom>> SearchAvailableRoomsAsync(
        string city, DateTime start, DateTime end, int persons,
        bool needPool, bool needRestaurant, int maxBeach, int maxCenter, bool sortByRating)
    {
        // sorteringen kan inte vara en @parameter, så vi väljer mellan två fasta texter (whitelist)
        string orderBy = sortByRating ? "hotels.rating DESC" : "rooms.price ASC";

        await using var command = new MySqlCommand(
            $"""
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
              AND (@needPool = 0 OR hotels.pool = 1)
              AND (@needRestaurant = 0 OR hotels.restaurant = 1)
              AND (@maxBeach = 0 OR hotels.distance_to_closest_beach <= @maxBeach)
              AND (@maxCenter = 0 OR hotels.distance_to_closest_city_center <= @maxCenter)
            ORDER BY {orderBy};
            """, connection);
        command.Parameters.AddWithValue("@city", city);
        command.Parameters.AddWithValue("@start", start);
        command.Parameters.AddWithValue("@end", end);
        command.Parameters.AddWithValue("@persons", persons);
        command.Parameters.AddWithValue("@needPool", needPool);
        command.Parameters.AddWithValue("@needRestaurant", needRestaurant);
        command.Parameters.AddWithValue("@maxBeach", maxBeach);
        command.Parameters.AddWithValue("@maxCenter", maxCenter);
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
