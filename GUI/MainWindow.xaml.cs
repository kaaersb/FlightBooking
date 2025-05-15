using GUI.Views;
using System.Windows;
using FlightBooking.Core.Data;
using FlightBooking.Core.Models;
using System.Runtime.CompilerServices;
using System.Configuration;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private User currentUser;
        private readonly IFlightRepository flightRepository = new FlightRepository(Config.ConnectionString);

        public ObservableCollection<Flight> Flights { get; } = new ObservableCollection<Flight>();

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        /* Funktion til når brugeren trykker på knappen "Hent fly"
         * 
         *
         */
        private async void btnGetFlights_Click(object sender, RoutedEventArgs e)
        {
            string origin = departureAirport.Text.Trim().ToUpper();
            string destination = arrivalAirport.Text.Trim().ToUpper();
            DateTime outbound;
            DateTime? inbound = null;

            if (string.IsNullOrEmpty(origin) || string.IsNullOrEmpty(destination))
            {
                MessageBox.Show("Fra og til skal udfyldes");
                return;
            }

            if(!datePickerOutbound.SelectedDate.HasValue)
            {
                MessageBox.Show("Vælg en afrejse dato");
            }

            outbound = datePickerOutbound.SelectedDate.Value;

            if (datePickerInbound.SelectedDate.HasValue)
            {
                inbound = datePickerInbound.SelectedDate.Value;
            }

            try
            {
                IEnumerable<Flight> flightResults = await flightRepository.SearchAsync(origin, destination, outbound, inbound);

                Flights.Clear();
                foreach (Flight flight in flightResults) 
                {
                    Flights.Add(flight);
                }
            }
            catch ( Exception ex )
            {
                MessageBox.Show($"Fejl ved hentning af fly: {ex.Message}");
            }
        }

        
        // event handler for createUser button
        private void OpenCreateUserWindow_Click(object sender, RoutedEventArgs e)
        {
            CreateUserView createWindow = new CreateUserView { Owner = this };

            if (createWindow.ShowDialog() == true)
            {
                User user = new User
                {
                    UserId = Guid.NewGuid(),
                    Name = createWindow.UserName,
                    Email = createWindow.Email,
                    Password = createWindow.Password
                };

                UserRepository userRepository = new UserRepository(Config.ConnectionString);
                userRepository.Add(user);

                MessageBox.Show($"Bruger: '{user.Name}' oprettet", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        
        private void OpenLoginViewWindow_Click(object sender, RoutedEventArgs e)
        {
            LoginView loginWindow = new LoginView { Owner = this };
            bool? ok = loginWindow.ShowDialog();

            if (ok == true)
            {
                currentUser = loginWindow.LoggedInUser;
                ApplyUserToUI();
            }
        }

        private void ApplyUserToUI()
        {
            login.Visibility = Visibility.Collapsed;
            createUser.Visibility = Visibility.Collapsed;

            ProfileButton.Visibility = Visibility.Visible;
            WelcomeText.Text = "Welcome, " + currentUser.Name;
            ProfileButton.Content = currentUser.Name;
        }

        private void OpenProfile_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}