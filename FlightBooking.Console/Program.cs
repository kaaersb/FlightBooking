using System;
using Microsoft.Extensions.Configuration;
using FlightBooking.Core.Data;
using FlightBooking.Core.Models;

class Program
{
    static void Main(string[] args)
    {
        // Hent connection string
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
        var connString = config.GetConnectionString("Default");

        // Opret repository
        IUserRepository repo = new UserRepository(connString);

        // Create
        /*
        var newUser = new User
        {
            UserId = Guid.NewGuid(),
            Name = "TestBruger",
            Email = "test@eksempel.dk",
            Password = "hemmelig"
        };
        repo.Add(newUser);
        Console.WriteLine($"Bruger oprettet med ID: {newUser.UserId}");

        // Find bruger med ID
        var hentet = repo.GetById(newUser.UserId);
        if (hentet != null)
        {
            Console.WriteLine($"Hentet fra DB: {hentet.Name}, {hentet.Email}");
        }

        // List alle brugere
        Console.WriteLine("\nAlle brugere:");
        foreach (var u in repo.GetAll())
        {
            Console.WriteLine($" - {u.UserId}: {u.Name} ({u.Email})");
        }

        

        User user = new User
        {
            UserId = Guid.NewGuid(),
            Name = "Nyt navn",
            Email = "nyemail@email.dk",
            Password = "hemmelig2"
        };

        repo.Add(user);
        
        // Opdater bruger
        User læst = repo.GetById(user.UserId);
        læst.Name = "Nyt navn 2";
        repo.Update(læst);

        Console.WriteLine("\nAlle brugere:");
        foreach (var u in repo.GetAll())
        {
            Console.WriteLine($" - {u.UserId}: {u.Name} ({u.Email})");
        }
        
        // Slet bruger
        repo.Delete(Guid.Parse(// BRUGER ID));

        */


        // Opret booking-repo
        IBookingRepository bookingRepo = new BookingRepository(connString);
        User newUser = new User
        {
            UserId = Guid.NewGuid(),
            Name = "Test",
            Email = "email@email.dk",
            Password = "password"
        };

        repo.Add(newUser);

        // Eksempel: Create
        var booking = new Booking
        {
            BookingId = Guid.NewGuid(),
            UserId = newUser.UserId
        };
        bookingRepo.Add(booking);
        Console.WriteLine($"Booking oprettet: {booking.BookingId} for user {booking.UserId}");

        // Eksempel: Read
        var hentetBooking = bookingRepo.GetById(booking.BookingId);
        Console.WriteLine($"Hentet booking for user: {hentetBooking?.UserId}");

        // Eksempel: List alle bookings for en user
        foreach (var b in bookingRepo.GetByUserId(newUser.UserId))
            Console.WriteLine($" - {b.BookingId}");

        // Eksempel: Delete
        //bookingRepo.Delete(booking.BookingId);
        //Console.WriteLine($"Booking {booking.BookingId} slettet");

    }
}
