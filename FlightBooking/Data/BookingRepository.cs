using System.Data;
using Microsoft.Data.SqlClient;
using FlightBooking.Core.Models;

namespace FlightBooking.Core.Data
{
    public class BookingRepository : IBookingRepository
    {
        private readonly string _conn;

        public BookingRepository(string connectionString)
        {
            _conn = connectionString;
        }

        public void Add(Booking booking)
        {
            const string sql = @"
                INSERT INTO Bookings (BookingId, UserId)
                VALUES (@BookingId, @UserId)";

            using var conn = new SqlConnection(_conn);
            conn.Open();

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@BookingId", SqlDbType.UniqueIdentifier).Value = booking.BookingId;
            cmd.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = booking.UserId;
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

            using var conn = new SqlConnection(_conn);
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
                SELECT BookingId, UserId
                  FROM Bookings
                 WHERE UserId = @UserId";

            using var conn = new SqlConnection(_conn);
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);

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

        public IEnumerable<Booking> GetAll()
        {
            const string sql = "SELECT BookingId, UserId FROM Bookings";
            using var conn = new SqlConnection(_conn);
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
            using var conn = new SqlConnection(_conn);
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@BookingId", bookingId);
            cmd.ExecuteNonQuery();
        }
    }
}
