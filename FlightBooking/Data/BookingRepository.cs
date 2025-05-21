using System.Data;
using Microsoft.Data.SqlClient;
using FlightBooking.Core.Models;

namespace FlightBooking.Core.Data
{
    public class BookingRepository : IBookingRepository
    {
        private readonly string _connectionString;

        public BookingRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Add(Booking booking)
        {
            const string sql = @"
                INSERT INTO Bookings (BookingId, UserId, OutboundFlightId, ReturnFlightId, BookingDate)
                VALUES (@BookingId, @UserId, @OutboundFlightId, @ReturnFlightId, @BookingDate)";

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@BookingId", SqlDbType.UniqueIdentifier).Value = booking.BookingId;
            cmd.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = booking.UserId;
            cmd.Parameters.Add("@OutboundFlightId", SqlDbType.UniqueIdentifier).Value = booking.OutboundFlightId;
            cmd.Parameters.Add("@ReturnFlightId", SqlDbType.UniqueIdentifier).Value =
                (object?)booking.ReturnFlightId ?? DBNull.Value;
            cmd.Parameters.Add("@BookingDate", SqlDbType.DateTime2).Value = booking.BookingDate;
            cmd.ExecuteNonQuery();
        }

        public Booking? GetById(Guid bookingId)
        {
            const string sql = @"
                SELECT b.BookingId, b.UserId,
                       u.UserId, u.Name, u.Email, u.Password
                  FROM Bookings b
                  JOIN Users u ON b.UserId = u.UserId
                 WHERE b.BookingId = @BookingId";

            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@BookingId", bookingId);

            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;

            return new Booking
            {
                BookingId = r.GetGuid(0),
                UserId = r.GetGuid(1),
                User = new User
                {
                    UserId = r.GetGuid(2),
                    Name = r.GetString(3),
                    Email = r.GetString(4),
                    Password = r.GetString(5)
                }
            };
        }

        public IEnumerable<Booking> GetByUserId(Guid userId)
        {
            const string sql = @"
        SELECT
            b.BookingId,
            b.UserId,
            b.BookingDate,

            ofl.FlightId      AS Outbound_FlightId,
            ofl.FlightNumber  AS Outbound_FlightNumber,
            ofl.Origin        AS Outbound_Origin,
            ofl.Destination   AS Outbound_Destination,
            ofl.DepartureUtc  AS Outbound_DepartureUtc,

            rfl.FlightId      AS Return_FlightId,
            rfl.FlightNumber  AS Return_FlightNumber,
            rfl.Origin        AS Return_Origin,
            rfl.Destination   AS Return_Destination,
            rfl.DepartureUtc  AS Return_DepartureUtc

        FROM Bookings b
        LEFT JOIN Flights ofl
          ON b.OutboundFlightId = ofl.FlightId
        LEFT JOIN Flights rfl
          ON b.ReturnFlightId   = rfl.FlightId
        WHERE b.UserId = @UserId";

            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);

            using var r = cmd.ExecuteReader();

            // Cache ordinals for speed and clarity
            var idx = new
            {
                BookingId = r.GetOrdinal("BookingId"),
                UserId = r.GetOrdinal("UserId"),
                BookingDate = r.GetOrdinal("BookingDate"),

                OfId = r.GetOrdinal("Outbound_FlightId"),
                OfNumber = r.GetOrdinal("Outbound_FlightNumber"),
                OfOrigin = r.GetOrdinal("Outbound_Origin"),
                OfDest = r.GetOrdinal("Outbound_Destination"),
                OfDepart = r.GetOrdinal("Outbound_DepartureUtc"),

                RfId = r.GetOrdinal("Return_FlightId"),
                RfNumber = r.GetOrdinal("Return_FlightNumber"),
                RfOrigin = r.GetOrdinal("Return_Origin"),
                RfDest = r.GetOrdinal("Return_Destination"),
                RfDepart = r.GetOrdinal("Return_DepartureUtc"),
            };

            while (r.Read())
            {
                var booking = new Booking
                {
                    BookingId = r.GetGuid(idx.BookingId),
                    UserId = r.GetGuid(idx.UserId),
                    BookingDate = r.GetDateTime(idx.BookingDate)
                };

                // Outbound flight (guard against NULL FK)
                if (!r.IsDBNull(idx.OfId))
                {
                    booking.OutboundFlight = new Flight
                    {
                        FlightId = r.GetGuid(idx.OfId),
                        FlightNumber = r.GetString(idx.OfNumber),
                        Origin = r.GetString(idx.OfOrigin),
                        Destination = r.GetString(idx.OfDest),
                        DepartureUtc = r.GetDateTime(idx.OfDepart)
                    };
                }

                // Return flight (optional)
                if (!r.IsDBNull(idx.RfId))
                {
                    booking.ReturnFlight = new Flight
                    {
                        FlightId = r.GetGuid(idx.RfId),
                        FlightNumber = r.GetString(idx.RfNumber),
                        Origin = r.GetString(idx.RfOrigin),
                        Destination = r.GetString(idx.RfDest),
                        DepartureUtc = r.GetDateTime(idx.RfDepart)
                    };
                }

                yield return booking;
            }
        }



        public IEnumerable<Booking> GetAll()
        {
            const string sql = "SELECT BookingId, UserId FROM Bookings";
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                yield return new Booking
                {
                    BookingId = r.GetGuid(0),
                    UserId = r.GetGuid(1)
                };
            }
        }

        public void Delete(Guid bookingId)
        {
            const string sql = "DELETE FROM Bookings WHERE BookingId = @BookingId";
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@BookingId", bookingId);
            cmd.ExecuteNonQuery();
        }
    }
}
