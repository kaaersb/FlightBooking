
namespace FlightBooking.Core.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;

        public bool Login()
        {
            Console.WriteLine("Login");
            return true;
        }

        public bool Register()
        {
            Console.WriteLine("Register");
            return true;
        }

        public User(string name, string email, string password)
        {
            Name = name;
            Email = email;
            Password = password;
        }

        public User()
        {

        }

        public override string ToString()
        {
            return $"Name: {Name}, Email: {Email}, Password: {Password}";
        }
    }
}
