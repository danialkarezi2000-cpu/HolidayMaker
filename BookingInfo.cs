public class BookingInfo
{
    // 1. Egenskaper som representerar informationen om en bokning.
    public long BookingId { get; }
    public string HotelName { get; }
    public string RoomName { get; }
    public DateTime Start { get; }
    public DateTime End { get; }
    public int Persons { get; }
    public decimal TotalPrice { get; }

    // 2. Konstruktor: tar emot 7 värden och sparar dem i egenskaperna.
    public BookingInfo(long bookingId, string hotelName, string roomName,
        DateTime start, DateTime end, int persons, decimal totalPrice)
    {
        BookingId = bookingId;
        HotelName = hotelName;
        RoomName = roomName;
        Start = start;
        End = end;
        Persons = persons;
        TotalPrice = totalPrice;
    }
}