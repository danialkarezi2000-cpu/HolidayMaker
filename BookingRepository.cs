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
            // Hänta kapacitet och pris för rummet.
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
            // om något går fel, rulla tillbaka transaktionen.
            await transaction.RollbackAsync();
            throw;
        }
    }
}
