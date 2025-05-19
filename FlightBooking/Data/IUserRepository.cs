using FlightBooking.Core.Models;

namespace FlightBooking.Core.Data
{
    public interface IUserRepository
    {
        void Add(User user);
        void Update(User user);
        void Delete(Guid guid);
        User? GetById(Guid id);
        User? GetByEmail(string email);
        IEnumerable<User> GetAll();
        (bool, string) ValidateUser(string email, string password);
    }
}
