using System;

namespace FlightBooking.Core.Models
{
    public class Booking
    {
        public Guid BookingId { get; set; }
        public Guid UserId { get; set; }
        public Guid OutboundFlightId { get; set; }
        public Guid? ReturnFlightId { get; set; }
        public Flight OutboundFlight { get; set; }
        public Flight? ReturnFlight { get; set; }
        public User User { get; set; }
        public DateTime BookingDate { get; set; }
        public decimal TotalPrice { get; set; }

    }
}
