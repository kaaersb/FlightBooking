using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FlyBooking.BLL;

namespace FlyBooking.UI
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }

        UserService userService = new UserService();
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            var (isValid, message) = userService.Validateuser(username, password);
            if(isValid)
            {
                this.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show(message);
            }
        }
        // denne Click funktion skal kalde en funktion i enten BLL eller MainWindow.xaml.cs som lukker/hider usercontrol
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
