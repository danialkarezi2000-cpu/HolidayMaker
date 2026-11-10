
CREATE DATABASE holiday_maker;
USE holiday_maker;
create table cities
(
    id   bigint auto_increment
        primary key,
    name varchar(120) not null,
    constraint cities_name_uq
        unique (name)
);

create table customers
(
    id        bigint auto_increment
        primary key,
    name      varchar(100) not null,
    last_name varchar(120) not null,
    email     varchar(180) not null,
    tlf       varchar(40)  not null,
    constraint customers_email_uq
        unique (email)
);

create table hotels
(
    id                              bigint auto_increment
        primary key,
    name                            varchar(115) not null,
    city_id                         bigint       not null,
    pool                            tinyint(1)   not null,
    restaurant                      tinyint(1)   not null,
    distance_to_closest_beach       int          not null,
    distance_to_closest_city_center int          not null,
    rating                          int          not null,
    constraint hotels_cities_id_fk
        foreign key (city_id) references cities (id),
    constraint hotels_beach_chk
        check (`distance_to_closest_beach` >= 0),
    constraint hotels_center_chk
        check (`distance_to_closest_city_center` >= 0),
    constraint hotels_rating_chk
        check (`rating` between 1 and 5)
);

create table rooms
(
    id       bigint auto_increment
        primary key,
    name     varchar(50)    not null,
    capacity int            not null,
    price    decimal(10, 2) not null,
    hotel_id bigint         not null,
    constraint rooms_hotel_name_uq
        unique (hotel_id, name),
    constraint rooms_hotels_id_fk
        foreign key (hotel_id) references hotels (id),
    constraint rooms_capacity_chk
        check (`capacity` > 0),
    constraint rooms_price_chk
        check (`price` > 0)
);

create table bookings
(
    id          bigint auto_increment
        primary key,
    customer_id bigint not null,
    room_id     bigint not null,
    start_date  date   not null,
    end_date    date   not null,
    num_persons int    not null,
    constraint bookings_customers_id_fk
        foreign key (customer_id) references customers (id),
    constraint bookings_rooms_id_fk
        foreign key (room_id) references rooms (id),
    constraint bookings_dates_chk
        check (`end_date` > `start_date`),
    constraint bookings_persons_chk
        check (`num_persons` > 0)
);

create index bookings_dates_idx
    on bookings (start_date, end_date);

