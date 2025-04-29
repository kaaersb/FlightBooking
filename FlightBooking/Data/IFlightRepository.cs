using FlightBooking.Core.Models;


namespace FlightBooking.Core.Data
{
    public interface IFlightRepository
    {
        Flight? GetById(Guid flightId);
        IEnumerable<Flight> GetAll();
        IEnumerable<Flight> Search();
    }
}
