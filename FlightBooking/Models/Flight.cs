using System;
using System.Collections.Generic;

namespace FlightBooking.Core.Models
{
    public class Flight
    {
        // Database-felter
        public Guid FlightId { get; set; }
        public string FlightNumber { get; set; } = null!;
        public string Origin { get; set; } = null!;
        public string Destination { get; set; } = null!;
        public DateTime DepartureUtc { get; set; }
        public DateTime ArrivalUtc { get; set; }
        public decimal Price { get; set; }
        public string Airline { get; set; } = null!;

        // PoC-felter fra SearchAPI JSON
        public string Type { get; set; } = "";        // "One way" eller "Round trip"
        public int TotalDuration { get; set; }             // i minutter
        public string BookingToken { get; set; } = "";        // token til booking API
        public List<string> Extensions { get; set; } = new();    // generelle extensions
        public string AirlineLogo { get; set; } = "";        // URL til logo
        public CarbonEmissions CarbonEmissions { get; set; } = new();    // emissioner

        // Hele ruten som segmenter (f.eks. outbound + inbound)
        public List<FlightSegment> FlightSegments { get; set; } = new();

        // Mellemlandinger
        public List<Layover> Layovers { get; set; } = new();
    }


    public class FlightSegment
    {
        public AirportInfo DepartureAirport { get; set; } = new();
        public AirportInfo ArrivalAirport { get; set; } = new();
        public int Duration { get; set; }              // i minutter
        public string Airplane { get; set; } = "";        // f.eks. "Airbus A320neo"
        public string Airline { get; set; } = "";        // f.eks. "Tap Air Portugal"
        public string FlightNumber { get; set; } = "";        // f.eks. "TP 757"
        public List<string> Extensions { get; set; } = new();    // specifikke extensions
    }

    public class AirportInfo
    {
        public string Id { get; set; } = "";   // IATA-kode, fx "CPH"
        public string Name { get; set; } = "";   // fx "Copenhagen Airport"
        public string Date { get; set; } = "";   // fx "2025-05-12"
        public string Time { get; set; } = "";   // fx "06:00"
    }

    public class Layover
    {
        public int Duration { get; set; }     // i minutter
        public string Name { get; set; } = ""; // fx "Humberto Delgado Airport"
        public string Id { get; set; } = ""; // fx "LIS"
    }

    public class CarbonEmissions
    {
        public int ThisFlight { get; set; }  // fx 508000 (g CO₂)
        public int TypicalForThisRoute { get; set; }  // fx 431000
        public int DifferencePercent { get; set; }  // fx 18
        public int LowestRoute { get; set; }  // fx 463000
    }
}
