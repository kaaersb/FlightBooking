using System.Text.Json;
using FlightBooking.Core.Models;


namespace FlightBooking.Core.Data
{
    public interface IFlightRepository
    {
        void Add(Flight flight);
        Task<IEnumerable<Flight>> SearchAsync(string origin, string destination, DateTime departureDate, DateTime? returnDate = null, int passengerCount = 1);
        Flight? ParseFlightElement(JsonElement flightElement);
    }
}
