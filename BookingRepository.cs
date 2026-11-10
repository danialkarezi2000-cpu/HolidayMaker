using MySqlConnector;

public class BookingRepository(MySqlConnection connection)
{
    // method som skapar en bokning i databasen och returnerar det nya bokningens id och totalpriset.
    public async Task<(long bookingId, decimal totalPrice)> CreateBookingAsync(
        long customerId, long roomId, DateTime start, DateTime end, int persons)
    {
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            // Hämta kapacitet och pris för rummet.
            await using var roomCommand = new MySqlCommand(
                "SELECT capacity, price FROM rooms WHERE id = @roomId FOR UPDATE;", connection, transaction);
            roomCommand.Parameters.AddWithValue("@roomId", roomId);
            int capacity;
            decimal price;
            await using (var reader = await roomCommand.ExecuteReaderAsync())
            {
                if (!await reader.ReadAsync())
                    throw new InvalidOperationException("Rummet finns inte.");
                capacity = reader.GetInt32("capacity");
                price = reader.GetDecimal("price");
            }

            // 2. Får alla plats?
            if (persons > capacity)
                throw new InvalidOperationException($"Rummet har bara plats för {capacity} personer.");

            // kolla om rummet är ledigt de datumen
            await using var checkCommand = new MySqlCommand(
                """
                SELECT COUNT(*) FROM bookings
                WHERE room_id = @roomId AND start_date < @end AND end_date > @start;
                """, connection, transaction);
            checkCommand.Parameters.AddWithValue("@roomId", roomId);
            checkCommand.Parameters.AddWithValue("@start", start);
            checkCommand.Parameters.AddWithValue("@end", end);
            long overlapping = Convert.ToInt64(await checkCommand.ExecuteScalarAsync());
            if (overlapping > 0)
                throw new InvalidOperationException("Rummet är redan bokat de datumen.");

            // allt är okej, så kan vi skapa bokningen.
            await using var insertCommand = new MySqlCommand(
                """
                INSERT INTO bookings (customer_id, room_id, start_date, end_date, num_persons)
                VALUES (@customerId, @roomId, @start, @end, @persons);
                """, connection, transaction);
            insertCommand.Parameters.AddWithValue("@customerId", customerId);
            insertCommand.Parameters.AddWithValue("@roomId", roomId);
            insertCommand.Parameters.AddWithValue("@start", start);
            insertCommand.Parameters.AddWithValue("@end", end);
            insertCommand.Parameters.AddWithValue("@persons", persons);
            await insertCommand.ExecuteNonQueryAsync();
            long bookingId = insertCommand.LastInsertedId;


            await transaction.CommitAsync();
            // beräkna totalpriset.
            int nights = (end - start).Days;
            return (bookingId, nights * price);
        }
        catch
        {
            // om något går fel, rulla tillbaka transaktionen och kasta vidare felet.
            await transaction.RollbackAsync();
            throw;
        }
    }

    // hämtar alla bokningar för en kund,hotellname,roomname,start,end date,antal personer och totalpriset
    public async Task<List<BookingInfo>> GetBookingsForCustomerAsync(long customerId)
    {
        await using var command = new MySqlCommand(
            """
            SELECT bookings.id, hotels.name AS hotel_name, rooms.name AS room_name,
                   bookings.start_date, bookings.end_date, bookings.num_persons,
                   DATEDIFF(bookings.end_date, bookings.start_date) * rooms.price AS total_price
            FROM bookings
            JOIN rooms ON bookings.room_id = rooms.id
            JOIN hotels ON rooms.hotel_id = hotels.id
            WHERE bookings.customer_id = @customerId
            ORDER BY bookings.start_date;
            """, connection);
        command.Parameters.AddWithValue("@customerId", customerId);
        await command.PrepareAsync();

        await using var reader = await command.ExecuteReaderAsync();
        var bookings = new List<BookingInfo>();
        while (await reader.ReadAsync())
        {
            bookings.Add(new BookingInfo(
                reader.GetInt64("id"), reader.GetString("hotel_name"), reader.GetString("room_name"),
                reader.GetDateTime("start_date"), reader.GetDateTime("end_date"),
                reader.GetInt32("num_persons"), reader.GetDecimal("total_price")));
        }
        return bookings;
    }

    // uppdaterar antal personer i en bokning, men bara om den är kundens egen och rummet har plats
    public async Task<bool> UpdatePersonsAsync(long bookingId, long customerId, int persons)
    {
        await using var command = new MySqlCommand(
            """
            UPDATE bookings
            JOIN rooms ON bookings.room_id = rooms.id
            SET bookings.num_persons = @persons
            WHERE bookings.id = @bookingId
              AND bookings.customer_id = @customerId
              AND rooms.capacity >= @persons;
            """, connection);
        command.Parameters.AddWithValue("@persons", persons);
        command.Parameters.AddWithValue("@bookingId", bookingId);
        command.Parameters.AddWithValue("@customerId", customerId);
        await command.PrepareAsync();
        int rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }

    // tar bort en bokning, men bara om den är kunden själv och returnerar true om det lyckades annars false.
    public async Task<bool> DeleteBookingAsync(long bookingId, long customerId)
    {
        await using var command = new MySqlCommand(
            "DELETE FROM bookings WHERE id = @bookingId AND customer_id = @customerId;", connection);
        command.Parameters.AddWithValue("@bookingId", bookingId);
        command.Parameters.AddWithValue("@customerId", customerId);
        await command.PrepareAsync();
        int rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }
}
