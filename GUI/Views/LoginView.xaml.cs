using System.Windows;
using System.Windows.Controls;
using FlightBooking.Core.Data;// Make sure this matches your namespace
using FlightBooking.Core.Models;
 

namespace GUI.Views
{
    public partial class LoginView : Window
    {
        private readonly static string connectionString = "Server=tcp:p2gruppe.database.windows.net,1433;Initial Catalog=flightbooking;Persist Security Info=False;User ID=p2gruppe;Password=admin123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"; // or read from config
        private readonly UserRepository _userRepository = new UserRepository(connectionString);

        public User LoggedInUser { get; private set; }

        public LoginView()
        {
            InitializeComponent();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            var (isValid, message) = _userRepository.ValidateUser(email, password);

            if (isValid)
            {
                LoggedInUser = _userRepository.GetByEmail(email);
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }


        }
    }

}