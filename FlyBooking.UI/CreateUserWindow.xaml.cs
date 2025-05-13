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
using System.Windows.Shapes;
using System.Xml.Linq;
using FlyBooking.BLL;

namespace FlyBooking.UI
{
    /// <summary>
    /// Interaction logic for CreateUserWindow.xaml
    /// </summary>
    public partial class CreateUserWindow : Window
    {
        public CreateUserWindow()
        {
            InitializeComponent();
        }

        //private void opretbutton_click(object sender, routedeventargs e)
        //{
        //    string username = usernametextbox.text;
        //    string password = passwordbox.password;

        //    /// validation logic. needs to be moved to the bll layer later.
        //    if (username == "admin" && password == "password")
        //    {
        //        messagebox.show("log in successful!");
        //        this.close(); // close the login window
        //    }
        //    else
        //    {
        //        messagebox.show("invalid username or password. hint: admin, password");
        //    }
        //}

        // UI Layer (e.g., a button click event)
        private void SaveUserButton_Click(object sender, EventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            UserService userService = new UserService(); // BLL
            bool result = userService.SaveUser(username, password);

            if (result)
            {
                MessageBox.Show("User saved successfully!");
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to save user.");
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Close the login window
        }
    }
}
