using FlightBooking.Core.Models;
using System.Text;
using System.Text.Json;

namespace FlightBooking.Core.Data
{
    public class FlightRepository : IFlightRepository
    {
        private readonly string _connectionString;
        private readonly HttpClient _http = new();
        private const string ApiKey = "z4qrWATZSi5qsEiQsL2arX6g";

       public FlightRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public FlightRepository() { }

        public async Task<IEnumerable<Flight>> SearchAsync(
            string origin,
            string destination,
            DateTime outboundDate,
            DateTime? returnDate = null)
        {
            var flightType = returnDate.HasValue ? "round_trip" : "one_way";
            var url = $"https://www.searchapi.io/api/v1/search" +
                      $"?api_key={ApiKey}" +
                      $"&departure_id={origin}" +
                      $"&arrival_id={destination}" +
                      $"&flight_type={flightType}" +
                      $"&outbound_date={outboundDate:yyyy-MM-dd}" +
                      $"&engine=google_flights" +
                      $"&currency=DKK";

            if (returnDate.HasValue)
                url += $"&return_date={returnDate:yyyy-MM-dd}";

            Console.Write(url);

            var json = await _http.GetStringAsync(url);
            using var doc = JsonDocument.Parse(json);

            List<Flight> list = new List<Flight>();
            foreach (var flight in doc.RootElement.GetProperty("best_flights").EnumerateArray())
            {
                // Parse basic felter
                var price = flight.GetProperty("price").GetDecimal();
                var type = flight.GetProperty("type").GetString()!;
                var dur = flight.GetProperty("total_duration").GetInt32();
                var token = flight.GetProperty("booking_token").GetString()!;
                var logo = flight.GetProperty("airline_logo").GetString()!;

                // Carbon emissions
                var ce = flight.GetProperty("carbon_emissions");
                var carbon = new CarbonEmissions
                {
                    ThisFlight = ce.GetProperty("this_flight").GetInt32(),
                    TypicalForThisRoute = ce.GetProperty("typical_for_this_route").GetInt32(),
                    DifferencePercent = ce.GetProperty("difference_percent").GetInt32(),
                    LowestRoute = ce.GetProperty("lowest_route").GetInt32()
                };

                // Segments
                var segments = new List<FlightSegment>();
                foreach (var seg in flight.GetProperty("flights").EnumerateArray())
                {
                    segments.Add(new FlightSegment
                    {
                        DepartureAirport = new AirportInfo
                        {
                            Id = seg.GetProperty("departure_airport").GetProperty("id").GetString()!,
                            Name = seg.GetProperty("departure_airport").GetProperty("name").GetString()!,
                            Date = seg.GetProperty("departure_airport").GetProperty("date").GetString()!,
                            Time = seg.GetProperty("departure_airport").GetProperty("time").GetString()!
                        },
                        ArrivalAirport = new AirportInfo
                        {
                            Id = seg.GetProperty("arrival_airport").GetProperty("id").GetString()!,
                            Name = seg.GetProperty("arrival_airport").GetProperty("name").GetString()!,
                            Date = seg.GetProperty("arrival_airport").GetProperty("date").GetString()!,
                            Time = seg.GetProperty("arrival_airport").GetProperty("time").GetString()!
                        },
                        Duration = seg.GetProperty("duration").GetInt32(),
                        Airplane = seg.GetProperty("airplane").GetString()!,
                        Airline = seg.GetProperty("airline").GetString()!,
                        FlightNumber = seg.GetProperty("flight_number").GetString()!,
                        Extensions = seg.GetProperty("extensions").EnumerateArray().Select(x => x.GetString()!).ToList()
                    });
                }

                // Layovers
                var layovers = new List<Layover>();
                if (flight.TryGetProperty("layovers", out var layEl) &&
                    layEl.ValueKind == JsonValueKind.Array)
                {
                    layovers = layEl.EnumerateArray()
                                    .Select(l => new Layover
                                    {
                                        Duration = l.GetProperty("duration").GetInt32(),
                                        Name = l.GetProperty("name").GetString()!,
                                        Id = l.GetProperty("id").GetString()!
                                    })
                                    .ToList();
                }

                // Afgrænsning af afgang/ankomst for databasen (eksempel: bruger første og sidste segment)
                var firstSeg = segments.First();
                var lastSeg = segments.Last();

                list.Add(new Flight
                {
                    FlightId = Guid.NewGuid(),
                    FlightNumber = firstSeg.FlightNumber,
                    Origin = firstSeg.DepartureAirport.Id,
                    Destination = lastSeg.ArrivalAirport.Id,
                    DepartureUtc = DateTime.Parse($"{firstSeg.DepartureAirport.Date} {firstSeg.DepartureAirport.Time}"),
                    ArrivalUtc = DateTime.Parse($"{lastSeg.ArrivalAirport.Date} {lastSeg.ArrivalAirport.Time}"),
                    Price = price,
                    Airline = firstSeg.Airline,

                    // PoC-felter
                    Type = type,
                    TotalDuration = dur,
                    BookingToken = token,
                    AirlineLogo = logo,
                    CarbonEmissions = carbon,
                    FlightSegments = segments,
                    Layovers = layovers,
                    Extensions = flight.GetProperty("extensions").EnumerateArray().Select(x => x.GetString()!).ToList()
                });
            }

            return list;
        }

        public Flight? GetById(Guid flightId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Flight> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
