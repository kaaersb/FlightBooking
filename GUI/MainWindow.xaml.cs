using GUI.Views;
using System.Windows;
using FlightBooking.Core.Data;
using FlightBooking.Core.Models;
using System.Runtime.CompilerServices;

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
            /*
            CreateUserWindow createUserWindow = new CreateUserWindow();
            createUserWindow.Owner = this; // Set the main window as the owner of the user creation window
            createUserWindow.ShowDialog(); // Show the user creation window as a dialog
            */

            MessageBox.Show(":D:D");
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
        }

        private void OpenProfile_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}