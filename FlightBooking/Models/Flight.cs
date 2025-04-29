using System;

namespace FlightBooking.Core.Models
{
    public class Flight
    {
        public Guid FlightId { get; set; }          // Primærnøgle
        public string FlightNumber { get; set; } = null!; // F.eks. "SK123"
        public string Origin { get; set; } = null!; // IATA-kode fx "CPH"
        public string Destination { get; set; } = null!; // IATA-kode fx "LHR"
        public DateTime DepartureUtc { get; set; }          // Afgangstidspunkt (UTC)
        public DateTime ArrivalUtc { get; set; }          // Ankomsttidspunkt (UTC)
        public decimal Price { get; set; }          // Pris i din valuta
        public string Airline { get; set; } = null!; // F.eks. "Scandinavian Airlines"
    }
}