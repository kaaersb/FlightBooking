using FlightBooking.Core.Models;

namespace FlightBooking.Core.Data
{
    public interface IBookingRepository
    {
        void Add(Booking booking);
        Booking? GetById(Guid bookingId);
        IEnumerable<Booking> GetByUserId(Guid userId);
        IEnumerable<Booking> GetAll();
        void Delete(Guid bookingId);
    }
}