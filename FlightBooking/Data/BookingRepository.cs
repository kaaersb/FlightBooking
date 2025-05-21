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

            ofl.FlightId      AS OutboundFlight_FlightId,
            ofl.FlightNumber  AS OutboundFlight_FlightNumber,
            ofl.Origin        AS OutboundFlight_Origin,
            ofl.Destination   AS OutboundFlight_Destination,

            rfl.FlightId      AS ReturnFlight_FlightId,
            rfl.FlightNumber  AS ReturnFlight_FlightNumber,
            rfl.Origin        AS ReturnFlight_Origin,
            rfl.Destination   AS ReturnFlight_Destination

        FROM Bookings b
        LEFT JOIN Flights ofl
          ON b.OutboundFlightId = ofl.FlightId
        LEFT JOIN Flights rfl
          ON b.ReturnFlightId = rfl.FlightId
        WHERE b.UserId = @UserId";

            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);

            using var r = cmd.ExecuteReader();
            // cache ordinals so we don’t call GetOrdinal in each loop
            var iBookingId = r.GetOrdinal("BookingId");
            var iUserId = r.GetOrdinal("UserId");
            var iDate = r.GetOrdinal("BookingDate");
            var iOfId = r.GetOrdinal("OutboundFlight_FlightId");
            var iOfNumber = r.GetOrdinal("OutboundFlight_FlightNumber");
            var iOfOrigin = r.GetOrdinal("OutboundFlight_Origin");
            var iOfDest = r.GetOrdinal("OutboundFlight_Destination");
            var iRfId = r.GetOrdinal("ReturnFlight_FlightId");
            var iRfNumber = r.GetOrdinal("ReturnFlight_FlightNumber");
            var iRfOrigin = r.GetOrdinal("ReturnFlight_Origin");
            var iRfDest = r.GetOrdinal("ReturnFlight_Destination");

            while (r.Read())
            {
                // Mandatory fields
                var booking = new Booking
                {
                    BookingId = r.GetGuid(iBookingId),
                    UserId = r.GetGuid(iUserId),
                    BookingDate = r.GetDateTime(iDate),
                };

                // Outbound flight: we *expect* this not to be null, but still guard it
                if (!r.IsDBNull(iOfId))
                {
                    booking.OutboundFlight = new Flight
                    {
                        FlightId = r.GetGuid(iOfId),
                        FlightNumber = r.GetString(iOfNumber),
                        Origin = r.GetString(iOfOrigin),
                        Destination = r.GetString(iOfDest)
                    };
                }

                // Return flight: stays null if user didn't book a return
                if (!r.IsDBNull(iRfId))
                {
                    booking.ReturnFlight = new Flight
                    {
                        FlightId = r.GetGuid(iRfId),
                        FlightNumber = r.GetString(iRfNumber),
                        Origin = r.GetString(iRfOrigin),
                        Destination = r.GetString(iRfDest)
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
