using System;

namespace FlightBooking.Core.Models
{
    public class Booking
    {
        public Guid BookingId { get; set; }
        public Guid UserId { get; set; }
        public Guid FlightId { get; set; }
        public Flight Flight { get; set; }
        public User User { get; set; }
        public DateTime BookingDate { get; set; }

    }
}
