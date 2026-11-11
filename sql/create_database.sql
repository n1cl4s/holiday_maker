drop database if exists holiday_maker;
create database holiday_maker;
use holiday_maker;

create table Cities
(
    id   int unsigned auto_increment
        primary key,
    name varchar(64) not null,
    constraint cities_name_unique
        unique (name)
);

create table Accomodations
(
    id                  int unsigned auto_increment
        primary key,
    name                varchar(64)  not null,
    city_id             int unsigned not null,
    pool                tinyint(1)   not null,
    restaurant          tinyint(1)   not null,
    distance_to_beach   int unsigned not null,
    distance_to_centrum int unsigned not null,
    rating              tinyint      not null,
    constraint Accomodations_name_city_unique
        unique (name, city_id),
    constraint Accomodations_Cities_id_fk
        foreign key (city_id) references Cities (id),
    constraint Accomodations_rating_limit
        check (`rating` between 1 and 5)
);

create table Customers
(
    id    int unsigned auto_increment
        primary key,
    name  varchar(64)  not null,
    email varchar(128) not null,
    tele  varchar(32)  not null,
    constraint Customers_email_unique
        unique (email),
    constraint Customers_tele_unique
        unique (tele)
);

create table Rooms
(
    id              int unsigned auto_increment
        primary key,
    name            varchar(64)  not null,
    accomodation_id int unsigned not null,
    capacity        int unsigned not null,
    price_per_night int unsigned not null,
    rooms_number    int unsigned not null,
    constraint Rooms_number_unique
        unique (rooms_number, accomodation_id),
    constraint Rooms_Accomodations_id_fk
        foreign key (accomodation_id) references Accomodations (id),
    constraint Rooms_capacity_range
        check (`capacity` between 1 and 5)
);

create table Bookings
(
    id            int unsigned auto_increment
        primary key,
    customer_id   int unsigned not null,
    room_id       int unsigned not null,
    booking_start date         not null,
    booking_end   date         not null,
    guests_count  int unsigned not null,
    constraint Booking_booking_date_unique
        unique (room_id, booking_start),
    constraint Bookings_Customers_id_fk
        foreign key (customer_id) references Customers (id),
    constraint Bookings_Rooms_id_fk
        foreign key (room_id) references Rooms (id),
    constraint Bookings_date_validation
        check (`booking_end` > `booking_start`),
    constraint Bookings_minimal_guests
        check (`guests_count` >= 1)
);

create view RoomDetails as
select r.id,
       a.name as hotel,
       c.name as city,
       r.rooms_number,
       r.capacity,
       r.price_per_night,
       a.rating,
       a.pool,
       a.restaurant,
       a.distance_to_beach,
       a.distance_to_centrum
from Rooms r
join Accomodations a on r.accomodation_id = a.id
join Cities c on a.city_id = c.id;

create user if not exists 'operator'@'%' identified by 'mintchokladglass';
grant select, insert, update, delete on holiday_maker.* to 'operator'@'%';