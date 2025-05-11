using FlightBooking.Core.Models;


namespace FlightBooking.Core.Data
{
    public interface IFlightRepository
    {
        Flight? GetById(Guid flightId);
        IEnumerable<Flight> GetAll();
        Task<IEnumerable<Flight>> SearchAsync(string origin, string destination, DateTime departureDate, DateTime? returnDate = null);
    }
}
