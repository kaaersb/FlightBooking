using System.Windows;
using System.Windows.Controls;
using FlightBooking.Core.Data; // Make sure this matches your namespace
 

namespace GUI.Views
{
    public partial class LoginView : Window
    {
        private readonly UserRepository _userRepository;

        public LoginView()
        {
            InitializeComponent();
            string connectionString = "Server=tcp:p2gruppe.database.windows.net,1433;Initial Catalog=flightbooking;Persist Security Info=False;User ID=p2gruppe;Password=admin123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"; // or read from config
            _userRepository = new UserRepository(connectionString);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            var (isValid, message) = _userRepository.ValidateUser(username, password);

            if (isValid)
            {
                this.Visibility = Visibility.Collapsed;
                // Optionally open the next window here
            }
            else
            {
                MessageBox.Show(message);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }

}