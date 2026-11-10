-- Holiday Maker: SQL-frågor (skrivna och testade 2026-11-07)
USE holiday_maker;

-- 1. Alla hotell med stadens namn (JOIN: FK city_id = PK id)
SELECT hotels.name, cities.name
FROM hotels
JOIN cities ON hotels.city_id = cities.id;

-- 2. Filtrera: bara hotell i Manila (gav 2 rader)
SELECT hotels.name, cities.name
FROM hotels
JOIN cities ON hotels.city_id = cities.id
WHERE cities.name = 'Manila';

-- 3. Sortera: högst rating först
SELECT hotels.name, cities.name, hotels.rating
FROM hotels
JOIN cities ON hotels.city_id = cities.id
ORDER BY hotels.rating DESC;

-- 4. Bokningar som krockar med perioden 3-7 december (gav 7 rader)
--    Regel: gammal start < ny slut OCH gammal slut > ny start
SELECT room_id, start_date, end_date
FROM bookings
WHERE start_date < '2026-12-07'
  AND end_date   > '2026-12-03';

-- 5. Lediga rum 3-7 december (alla rum UTOM de som krockar)
SELECT id, name, capacity, price, hotel_id
FROM rooms
WHERE id NOT IN (
    SELECT room_id
    FROM bookings
    WHERE start_date < '2026-12-07'
      AND end_date   > '2026-12-03'
);

-- 6. Lediga rum 3-7 dec för minst 3 personer (gav 9 rader)
SELECT id, name, capacity, price, hotel_id
FROM rooms
WHERE id NOT IN (
    SELECT room_id
    FROM bookings
    WHERE start_date < '2026-12-07'
      AND end_date   > '2026-12-03'
)
  AND capacity >= 3;

-- 7. HELA SÖKNINGEN: lediga rum 3-7 dec, minst 3 personer, i Manila (gav 4 rader)
--    rooms -> hotels (hotel_id = id) -> cities (city_id = id)
SELECT rooms.id, rooms.name, rooms.capacity, rooms.price, hotels.name
FROM rooms
JOIN hotels ON rooms.hotel_id = hotels.id
JOIN cities ON hotels.city_id = cities.id
WHERE rooms.id NOT IN (
    SELECT room_id
    FROM bookings
    WHERE start_date < '2026-12-07'
      AND end_date   > '2026-12-03'
)
  AND rooms.capacity >= 3
  AND cities.name = 'Manila';