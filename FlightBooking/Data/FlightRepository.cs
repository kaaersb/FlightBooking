using FlightBooking.Core.Models;
using Microsoft.Data.SqlClient;
using System.Text;
using System.Text.Json;

namespace FlightBooking.Core.Data
{
    public class FlightRepository : IFlightRepository
    {
        private readonly string _connectionString;
        private readonly HttpClient _http = new();
        private const string ApiKey = "API-NØGLE";
        public FlightRepository() { }

        public FlightRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Flight>> SearchAsync(
            string origin,
            string destination,
            DateTime outboundDate,
            DateTime? returnDate = null)
        {
            string flightType = returnDate.HasValue ? "round_trip" : "one_way";
            string url = $"https://www.searchapi.io/api/v1/search" +
                         $"?api_key={ApiKey}" +
                         $"&departure_id={origin}" +
                         $"&arrival_id={destination}" +
                         $"&flight_type={flightType}" +
                         $"&outbound_date={outboundDate:yyyy-MM-dd}" +
                         $"&engine=google_flights" +
                         $"&currency=DKK";

            if (returnDate.HasValue)
            {
                url += $"&return_date={returnDate:yyyy-MM-dd}";
            }

            Console.WriteLine(url);

            string json = await _http.GetStringAsync(url);
            JsonDocument doc = JsonDocument.Parse(json);
            List<Flight> list = new List<Flight>();

            string[] flightGroups = { "best_flights", "other_flights" };

            foreach (string group in flightGroups)
            {
                if (!doc.RootElement.TryGetProperty(group, out JsonElement flightArray) ||
                    flightArray.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }

                foreach (JsonElement flightElement in flightArray.EnumerateArray())
                {
                    Flight? resultFlight = ParseFlightElement(flightElement);
                    if (resultFlight != null)
                    {
                        list.Add(resultFlight);
                    }
                }
            }

            list = list.OrderBy(f => f.Price).ToList(); 

            return list;
        }

        private Flight? ParseFlightElement(JsonElement flightElement)
        {
            decimal price = flightElement.TryGetProperty("price", out JsonElement priceEl) && priceEl.TryGetDecimal(out decimal p) ? p : 0;
            string type = flightElement.TryGetProperty("type", out JsonElement typeEl) ? typeEl.GetString() ?? "" : "";
            int duration = flightElement.TryGetProperty("total_duration", out JsonElement durEl) ? durEl.GetInt32() : 0;
            string token = flightElement.TryGetProperty("booking_token", out JsonElement tokenEl) ? tokenEl.GetString() ?? "" : "";
            string logo = flightElement.TryGetProperty("airline_logo", out JsonElement logoEl) ? logoEl.GetString() ?? "" : "";

            CarbonEmissions carbon = new CarbonEmissions();
            if (flightElement.TryGetProperty("carbon_emissions", out JsonElement ceEl))
            {
                carbon.ThisFlight = ceEl.TryGetProperty("this_flight", out JsonElement tf) ? tf.GetInt32() : 0;
                carbon.TypicalForThisRoute = ceEl.TryGetProperty("typical_for_this_route", out JsonElement tr) ? tr.GetInt32() : 0;
                carbon.DifferencePercent = ceEl.TryGetProperty("difference_percent", out JsonElement dp) ? dp.GetInt32() : 0;
                carbon.LowestRoute = ceEl.TryGetProperty("lowest_route", out JsonElement lr) ? lr.GetInt32() : 0;
            }

            List<FlightSegment> segments = new List<FlightSegment>();
            if (!flightElement.TryGetProperty("flights", out JsonElement segmentsEl) ||
                segmentsEl.ValueKind != JsonValueKind.Array)
            {
                return null;
            }

            foreach (JsonElement segEl in segmentsEl.EnumerateArray())
            {
                FlightSegment segment = new FlightSegment
                {
                    DepartureAirport = new AirportInfo
                    {
                        Id = segEl.GetProperty("departure_airport").GetProperty("id").GetString() ?? "",
                        Name = segEl.GetProperty("departure_airport").GetProperty("name").GetString() ?? "",
                        Date = segEl.GetProperty("departure_airport").GetProperty("date").GetString() ?? "",
                        Time = segEl.GetProperty("departure_airport").GetProperty("time").GetString() ?? ""
                    },
                    ArrivalAirport = new AirportInfo
                    {
                        Id = segEl.GetProperty("arrival_airport").GetProperty("id").GetString() ?? "",
                        Name = segEl.GetProperty("arrival_airport").GetProperty("name").GetString() ?? "",
                        Date = segEl.GetProperty("arrival_airport").GetProperty("date").GetString() ?? "",
                        Time = segEl.GetProperty("arrival_airport").GetProperty("time").GetString() ?? ""
                    },
                    Duration = segEl.TryGetProperty("duration", out JsonElement durSeg) ? durSeg.GetInt32() : 0,
                    Airplane = segEl.TryGetProperty("airplane", out JsonElement air) ? air.GetString() ?? "" : "",
                    Airline = segEl.TryGetProperty("airline", out JsonElement line) ? line.GetString() ?? "" : "",
                    FlightNumber = segEl.TryGetProperty("flight_number", out JsonElement fn) ? fn.GetString() ?? "" : "",
                    Extensions = segEl.TryGetProperty("extensions", out JsonElement ext) && ext.ValueKind == JsonValueKind.Array
                        ? ext.EnumerateArray().Select(x => x.GetString() ?? "").ToList()
                        : new List<string>()
                };

                segments.Add(segment);
            }

            List<Layover> layovers = new List<Layover>();
            if (flightElement.TryGetProperty("layovers", out JsonElement layEl) &&
                layEl.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement layover in layEl.EnumerateArray())
                {
                    layovers.Add(new Layover
                    {
                        Duration = layover.TryGetProperty("duration", out JsonElement dur) ? dur.GetInt32() : 0,
                        Name = layover.TryGetProperty("name", out JsonElement name) ? name.GetString() ?? "" : "",
                        Id = layover.TryGetProperty("id", out JsonElement id) ? id.GetString() ?? "" : ""
                    });
                }
            }

            if (segments.Count == 0)
            {
                return null;
            }

            FlightSegment firstSeg = segments.First();
            FlightSegment lastSeg = segments.Last();

            DateTime departureUtc = DateTime.TryParse($"{firstSeg.DepartureAirport.Date} {firstSeg.DepartureAirport.Time}", out DateTime dep) ? dep : DateTime.MinValue;
            DateTime arrivalUtc = DateTime.TryParse($"{lastSeg.ArrivalAirport.Date} {lastSeg.ArrivalAirport.Time}", out DateTime arr) ? arr : DateTime.MinValue;

            List<string> extensions = flightElement.TryGetProperty("extensions", out JsonElement extRoot) && extRoot.ValueKind == JsonValueKind.Array
                ? extRoot.EnumerateArray().Select(x => x.GetString() ?? "").ToList()
                : new List<string>();

            return new Flight
            {
                FlightId = Guid.NewGuid(),
                FlightNumber = firstSeg.FlightNumber,
                Origin = firstSeg.DepartureAirport.Id,
                Destination = lastSeg.ArrivalAirport.Id,
                DepartureUtc = departureUtc,
                ArrivalUtc = arrivalUtc,
                Price = price,
                Airline = firstSeg.Airline,
                Type = type,
                TotalDuration = duration,
                BookingToken = token,
                AirlineLogo = logo,
                CarbonEmissions = carbon,
                FlightSegments = segments,
                Layovers = layovers,
                Extensions = extensions
            };
        }

        public void Add(Flight flight)
        {
            const string sql = @"
        INSERT INTO Flights (FlightId, FlightNumber, Origin, Destination, DepartureUtc, ArrivalUtc, Price, Airline)
        VALUES (@FlightId, @FlightNumber, @Origin, @Destination, @DepartureUtc, @ArrivalUtc, @Price, @Airline)";

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@FlightId", flight.FlightId);
            cmd.Parameters.AddWithValue("@FlightNumber", flight.FlightNumber);
            cmd.Parameters.AddWithValue("@Origin", flight.Origin);
            cmd.Parameters.AddWithValue("@Destination", flight.Destination);
            cmd.Parameters.AddWithValue("@DepartureUtc", flight.DepartureUtc);
            cmd.Parameters.AddWithValue("@ArrivalUtc", flight.ArrivalUtc);
            cmd.Parameters.AddWithValue("@Price", flight.Price);
            cmd.Parameters.AddWithValue("@Airline", flight.Airline);

            cmd.ExecuteNonQuery();
        }
    }
}
