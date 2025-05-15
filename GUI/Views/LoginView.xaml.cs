using System.Windows;
using System.Windows.Controls;
using FlightBooking.Core.Data;// Make sure this matches your namespace
using FlightBooking.Core.Models;
using System.Configuration;


namespace GUI.Views
{
    public partial class LoginView : Window
    {
        private readonly UserRepository _userRepository = new UserRepository(Config.ConnectionString);

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