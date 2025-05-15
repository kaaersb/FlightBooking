using System.Data;
using Microsoft.Data.SqlClient;
using FlightBooking.Core.Models;

namespace FlightBooking.Core.Data
{
    public class UserRepository : IUserRepository
    {
        // FORBINDELSES STRENG TIL DATABASE
        private readonly string _connectionString;


        // LAV INSTANS AF KLASSE MED FORBINDELSES STRENG
        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // ---------------------------------------------------------------
        // --------------- CREATE READ UPDATE DELETE ---------------------
        // ---------------------------------------------------------------

        // FUNKTION TIL AT TILFØJE EN BRUGER TIL DATABASEN
        public void Add(User user)
        {
            const string sql = @"
                INSERT INTO Users (UserId, Name, Email, Password)
                VALUES (@UserId, @Name, @Email, @Password)";

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = user.UserId;
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 200).Value = user.Name;
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 200).Value = user.Email;
            cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 200).Value = user.Password;

            cmd.ExecuteNonQuery();
        }


        // FUNKTIONEN TIL AT OPDATERE BRUGER I DATABASEN
        public void Update(User user)
        {
            const string sql = @"
                UPDATE Users
                   SET Name     = @Name,
                       Email    = @Email,
                       Password = @Password
                 WHERE UserId  = @UserId";

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@UserId", user.UserId);
            cmd.Parameters.AddWithValue("@Name", user.Name);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Password", user.Password);
            cmd.ExecuteNonQuery();

        }


        // FUNKTION TIL AT SLETTE EN BRUGER I DATABASEN
        public void Delete(Guid id)
        {
            const string sql = "DELETE FROM Users WHERE UserId = @UserId";

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@UserId", id);
            cmd.ExecuteNonQuery();
            Console.WriteLine($"Slettet bruger med ID: {id}");
        }


        // FUNKTION TIL AT HENTE EN BRUGER FRA DATABASEN
        public User? GetById(Guid id)
        {
            const string sql = @"
                SELECT UserId, Name, Email, Password
                  FROM Users
                 WHERE UserId = @UserId";

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@UserId", id);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new User
            {
                UserId = reader.GetGuid(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                Password = reader.GetString(3)
            };
        }

        public User? GetByEmail(string email)
        {
            const string sql = @"
        SELECT UserId, Name, Email, Password
          FROM Users
         WHERE Email = @Email";

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 200).Value = email;

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return null;

            return new User
            {
                UserId = reader.GetGuid(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                Password = reader.GetString(3)
            };
        }


        // FUNKTION TIL AT HENTE ALLE BRUGERE FRA DATABASEN
        public IEnumerable<User> GetAll()
        {
            const string sql = "SELECT UserId, Name, Email, Password FROM Users";

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                yield return new User
                {
                    UserId = reader.GetGuid(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2),
                    Password = reader.GetString(3)
                };
            }
        }

        public (bool, string) ValidateUser(string email, string password)
        {
            const string sql = "SELECT Password FROM Users WHERE Email = @Email";

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Email", email);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return (false, "User not found.");

            var storedPassword = reader.GetString(0);
            return (storedPassword == password)
                ? (true, "Login successful!")
                : (false, "Invalid password.");
        }
    }
}
