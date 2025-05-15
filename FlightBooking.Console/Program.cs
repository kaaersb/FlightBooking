using System;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using FlightBooking.Core.Data;
using FlightBooking.Core.Models;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Hent connection string
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
        var connectionString = config.GetConnectionString("Default");

        Console.WriteLine(connectionString);

        // Opret repositories
        IUserRepository userRepo = new UserRepository(connectionString);
        IFlightRepository flightRepo = new FlightRepository(connectionString);
        IBookingRepository bookingRepo = new BookingRepository(connectionString);


        
        Console.WriteLine("=== USER CRUD ===");
        // CREATE
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Name = "Alice",
            Email = "alice@example.com",
            Password = "secret"
        };
        userRepo.Add(user);
        Console.WriteLine($"Created User: {user.UserId}");

        // READ
        var fetchedUser = userRepo.GetById(user.UserId);
        Console.WriteLine($"Read User: {fetchedUser?.Name}, {fetchedUser?.Email}");

        // LIST
        Console.WriteLine("All Users:");
        foreach (var u in userRepo.GetAll())
            Console.WriteLine($" - {u.UserId}: {u.Name} ({u.Email})");

        // UPDATE
        user.Name = "Alice Updated";
        userRepo.Update(user);
        Console.WriteLine($"Updated User: {user.UserId}");

        // VERIFY UPDATE
        Console.WriteLine("All Users After Update:");
        foreach (var u in userRepo.GetAll())
            Console.WriteLine($" - {u.UserId}: {u.Name} ({u.Email})");

        // DELETE
        userRepo.Delete(user.UserId);
        Console.WriteLine($"Deleted User: {user.UserId}");

        // VERIFY DELETE
        Console.WriteLine("All Users After Delete:");
        foreach (var u in userRepo.GetAll())
            Console.WriteLine($" - {u.UserId}: {u.Name} ({u.Email})");


        Console.WriteLine("\n=== FLIGHT CRUD ===");
        // CREATE
        var flight = new Flight
        {
            FlightId = Guid.NewGuid(),
            FlightNumber = "SK123",
            Origin = "CPH",
            Destination = "LHR",
            DepartureUtc = DateTime.UtcNow.AddDays(1).AddHours(9),
            ArrivalUtc = DateTime.UtcNow.AddDays(1).AddHours(11),
            Price = 199.50m,
            Airline = "SAS"
        };
        Console.WriteLine($"Created Flight: {flight.FlightId}");

        string origin = "CPH";
        string destination = "ARN";
        DateTime outboundDate = new DateTime(2025, 6, 1);
        DateTime? returnDate = new DateTime(2025, 6, 8);

        Console.WriteLine($"Søger efter fly mellem {origin} og {destination}");

        IEnumerable<Flight> flights = await flightRepo.SearchAsync(origin, destination, outboundDate, returnDate);

        Console.WriteLine($"Found {flights?.Count() ?? 0} flights:");

        foreach(Flight f in flights)
        {
            Console.WriteLine($"Flight ID: {f.FlightId}, From: {f.Origin} To: {f.Destination}, Departure: {f.DepartureUtc}, Price: {f.Price:C}");
        }

        Console.WriteLine("\n=== BOOKING CRUD ===");
        // Sørg for at have en gyldig userId og flightId; vi genopretter en test‐user
        var bookUser = new User
        {
            UserId = Guid.NewGuid(),
            Name = "Bob",
            Email = "bob@example.com",
            Password = "pass"
        };
        userRepo.Add(bookUser);

        var booking = new Booking
        {
            BookingId = Guid.NewGuid(),
            UserId = bookUser.UserId,
            OutboundFlightId = flight.FlightId
        };
        bookingRepo.Add(booking);
        Console.WriteLine($"Created Booking: {booking.BookingId} for User {booking.UserId}");

        // READ
        var fetchedBooking = bookingRepo.GetById(booking.BookingId);
        Console.WriteLine($"Read Booking: {fetchedBooking?.BookingId}, User {fetchedBooking?.UserId}");

        // LIST FOR USER
        Console.WriteLine($"All Bookings for User {bookUser.UserId}:");
        foreach (var b in bookingRepo.GetByUserId(bookUser.UserId))
            Console.WriteLine($" - {b.BookingId}");

        // DELETE
        bookingRepo.Delete(booking.BookingId);
        Console.WriteLine($"Deleted Booking: {booking.BookingId}");

        // Ryd op: slet test‐user
        userRepo.Delete(bookUser.UserId);


        // Lave en booking med fly og user
        Console.WriteLine("=========== BOOKING MED BRUGER OG FLY ==========");
        if (flights.Any())
        {
            Console.WriteLine("Der er fly i listen");
            Flight selectedFlight = flights.First();

            User userTest = userRepo.GetById(Guid.Parse("1e2b1c77-ac87-444d-b322-debb38b69d6a"));
            if (user == null)
            {
                Console.WriteLine("Ingen bruger >:(");
            }

            Booking bookingTest = new Booking
            {
                BookingId = Guid.NewGuid(),
                UserId = userTest.UserId,
                OutboundFlightId = selectedFlight.FlightId,
                OutboundFlight = selectedFlight,
                User = userTest,
                BookingDate = DateTime.Now
            };

            flightRepo.Add(selectedFlight);
            bookingRepo.Add(bookingTest);

            Console.WriteLine("Booking skabt! User: " + userTest.Name + ", Flight: " + selectedFlight.FlightNumber);
        }

        else
        {
            Console.WriteLine("Ingen fly :((");
        }

        Console.WriteLine("\n--- ALL TESTS FÆRDIGE ---");
        
    }
}