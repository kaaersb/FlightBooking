using GUI.Views;
using System.Windows;
using FlightBooking.Core.Data;
using FlightBooking.Core.Models;
using System.Runtime.CompilerServices;
using System.Configuration;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private User currentUser;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;
        }

        /* Funktion til når brugeren trykker på knappen "Hent fly"
         * 
         *
         */
        private async void btnGetFlights_Click(object sender, RoutedEventArgs e)
        {

            MessageBox.Show("Hent nogle fly :D");
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