using FlyBooking.UI;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly FlightService _flightService;

        // Liste til at binde til UI
        public ObservableCollection<FlightGroup> FlightGroups { get; set; }

        CreateUserWindow createUserWindow = new CreateUserWindow();

        public MainWindow()
        {
            InitializeComponent();

            // Initialisere FlightService objekt
            _flightService = new FlightService();

            // Initialiser ObservableCollection
            FlightGroups = new ObservableCollection<FlightGroup>();

            DataContext = this;
        }

        /* Funktion til når brugeren trykker på knappen "Hent fly"
         * 
         *
         */
        private async void btnGetFlights_Click(object sender, RoutedEventArgs e)
        {
            string engine = "google_flights";
            string flight_type = "round_trip";

            string departure_id = "";
            departure_id = departureAirport.Text.ToUpper().Trim();

            string arrival_id = "";
            arrival_id = arrivalAirport.Text.ToUpper().Trim();

            if (string.IsNullOrEmpty(departure_id) || string.IsNullOrEmpty(arrival_id))
            {
                return;
            }

            string outbound_date = "";
            DateTime? selectedDateOutbound = datePickerOutbound.SelectedDate;
            if (selectedDateOutbound.HasValue)
            {
                outbound_date = selectedDateOutbound.Value.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            }

            string return_date = "";
            DateTime? selectedDateInbound = datePickerInbound.SelectedDate;
            if (selectedDateInbound.HasValue)
            {
                return_date = selectedDateInbound.Value.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            }

            // Knappen venter på at addFlightsToList er færdig
            await addFlightsToList(engine, flight_type, departure_id, arrival_id, outbound_date, return_date);

        }

        /* Metode til at tilføje fly til listen der vises for brugeren
         * 
        */
        private async Task addFlightsToList(string engine, string flight_type, string departure_id, string arrival_id, string outbound_date, string return_date)
        {
            try
            {
                // Hent fly data fra API
                var flightData = await _flightService.GetFlightsAsync(engine, flight_type, departure_id, arrival_id, outbound_date, return_date);

                // Hvis der ikke kommer noget data fra API'et, return
                if (flightData == null || (flightData.BestFlights == null && flightData.OtherFlights == null))
                {
                    MessageBox.Show("Ingen flydata modtaget.");
                    return;
                }

                // Sørg for at listen af fly er tom
                FlightGroups.Clear();

                // Tilføj fly fra "BestFlights" listen
                // BestFlights er en gruppering fra Google/API'et
                if (flightData.BestFlights != null)
                {
                    foreach (var group in flightData.BestFlights)
                    {
                        FlightGroups.Add(group);
                    }
                }

                // Samme fra ovenstående men med OtherFlights
                if (flightData.OtherFlights != null)
                {
                    foreach (var group in flightData.OtherFlights)
                    {
                        FlightGroups.Add(group);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fejl ved hentning af flydata: " + ex.Message);
            }
        }


        // event handler for createUser button
        private void OpenCreateUserWindow_Click(object sender, RoutedEventArgs e)
        {
            CreateUserWindow createUserWindow = new CreateUserWindow();
            createUserWindow.Owner = this; // Set the main window as the owner of the user creation window
            createUserWindow.ShowDialog(); // Show the user creation window as a dialog
        }

        private void OpenLoginViewWindow_Click(object sender, RoutedEventArgs e)
        {
            LoginView loginWindow = new LoginView();
            loginWindow.Owner = this;
            loginWindow.ShowDialog();
        }
    }
}