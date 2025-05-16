using GUI.Views;
using System.Windows;
using FlightBooking.Core.Data;
using FlightBooking.Core.Models;
using System.Runtime.CompilerServices;
using System.Configuration;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;

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
            // 1) Read & normalize IATA codes
            string origin = originTextbox.Text.Trim().ToUpperInvariant();
            string destination = destinationTextbox.Text.Trim().ToUpperInvariant();

            if (origin.Length != 3 || destination.Length != 3)
            {
                MessageBox.Show("Fra og til skal være 3‐bogstavers IATA‐koder");
                return;
            }

            // 2) Read outbound date
            if (!OutboundDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Vælg en afrejsedato");
                return;
            }
            DateTime outboundDate = OutboundDate.SelectedDate.Value;

            // 3) Are we doing round trip?
            bool isRoundTrip = RoundTripTab.IsSelected;

            // 4) Read return date only if round‐trip
            DateTime? returnDate = null;
            if (isRoundTrip)
            {
                if (!ReturnDate.SelectedDate.HasValue)
                {
                    MessageBox.Show("Vælg en returdato eller skift til enkelt tur");
                    return;
                }
                returnDate = ReturnDate.SelectedDate.Value;
            }

            // 5) Read passenger count
            int passengerCount = 1; // default
            if (PassengerCountBox.SelectedItem is ComboBoxItem item &&
                int.TryParse(item.Content.ToString(), out var pc))
            {
                passengerCount = pc;
            }
            else
            {
                MessageBox.Show("Vælg antal rejsende");
                return;
            }

            // 6) Call your updated repository method
            try
            {
                var flightResults = await flightRepository
                    .SearchAsync(origin, destination, outboundDate, returnDate, passengerCount);

                Flights.Clear();
                foreach (var f in flightResults)
                    Flights.Add(f);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fejl ved hentning af fly: {ex.Message}");
            }
        }


        // event handle for createUser button
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

        private void BookFlight_Click(object sender, RoutedEventArgs e)
        {
            if (currentUser == null)
            {
                var result = MessageBox.Show("Du skal være logget ind for at booke. Har du en bruger?",
                                             "Log ind påkrævet",
                                             MessageBoxButton.YesNoCancel,
                                             MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    LoginView loginView = new LoginView { Owner = this };
                    if (loginView.ShowDialog() != true)
                    {
                        return;
                    }
                    currentUser = loginView.LoggedInUser;
                    ApplyUserToUI();
                }
                else if (result == MessageBoxResult.No)
                {
                    CreateUserView createUserView = new CreateUserView { Owner = this };
                    if(createUserView.ShowDialog() != true)
                    {
                        return;
                    }
                    User newUser = new User
                    {
                        UserId = Guid.NewGuid(),
                        Name = createUserView.Name,
                        Email = createUserView.Email,
                        Password = createUserView.Password
                    };

                    new UserRepository(Config.ConnectionString).Add(newUser);
                    currentUser = newUser;
                    ApplyUserToUI();

                }
                else
                {
                    return;
                }
            }

            if((sender as FrameworkElement)?.DataContext is not Flight flight) {

                return;
            }

            Booking booking = new Booking
            {
                BookingId = Guid.NewGuid(),
                UserId = currentUser.UserId,
                OutboundFlightId = flight.FlightId,
                ReturnFlightId = null,
                BookingDate = DateTime.Now,
                OutboundFlight = flight,
                ReturnFlight = null,
                User = currentUser
            };

            try
            {
                BookingRepository bookingRepository = new BookingRepository(Config.ConnectionString);
                bookingRepository.Add(booking);
                MessageBox.Show("Booking gennemført!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Booking kunne ikke gennemføres: {ex.Message}", "Fejl", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }


    }
}